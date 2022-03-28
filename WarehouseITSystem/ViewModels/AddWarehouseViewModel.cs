using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using WarehouseITSystem.Models;

namespace WarehouseITSystem.ViewModels
{
    internal class AddWarehouseViewModel : Screen
    {
        public AddWarehouseViewModel() : base() 
        {
            using WarehouseContext db = new();
            Types = db.WarehouseTypes.Select(wt => wt.Name).ToList();
        }

        public AddWarehouseViewModel(WarehouseToScreen warehouse) : base()
        {
            using WarehouseContext db = new();
            oldWarehouse = warehouse;
            Address = warehouse.Address ?? "";
            Phone = warehouse.Phone ?? "";
            SelectedType = warehouse.WarehouseType;
            Types = db.WarehouseTypes.Select(wt => wt.Name).ToList();            
        }

        private readonly WarehouseToScreen? oldWarehouse;

        private string address = "";
        public string Address
        {
            get => address;
            set
            {
                address = value;
                NotifyOfPropertyChange(() => Address);
                NotifyOfPropertyChange(() => CanAddWarehouse);
            }
        }

        private string phone = "";
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                NotifyOfPropertyChange(() => Phone);
                NotifyOfPropertyChange(() => CanAddWarehouse);
            }
        }

        private string? selectedType;
        public string? SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                NotifyOfPropertyChange(() => SelectedType);
                NotifyOfPropertyChange(() => CanAddWarehouse);
                NotifyOfPropertyChange(() => CanDeleteType);
                NotifyOfPropertyChange(() => CanChangeType);
            }
        }

        private ICollection<string?> types = new List<string?>();
        public ICollection<string?> Types
        {
            get => types;
            set
            {
                types = value;
                NotifyOfPropertyChange(() => Types);
                NotifyOfPropertyChange(() => CanAddWarehouse);
            }
        }

        public async void AddType()
        {
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddWarehouseTypeViewModel());
            if (response.GetValueOrDefault(false))
            {
                using WarehouseContext db = new();
                Types = db.WarehouseTypes.Select(wt => wt.Name).ToList();
            }
        }

        public bool CanDeleteType
        {
            get { return SelectedType != null; }
        }
        public async void DeleteType()
        {
            using WarehouseContext db = new();
            Types.Remove(SelectedType!);
            db.WarehouseTypes.Remove(db.WarehouseTypes.Where(p => p.Name == SelectedType!).FirstOrDefault()!);
            await db.SaveChangesAsync();

            Types = db.WarehouseTypes.Select(wt => wt.Name).ToList();
            SelectedType = Types.FirstOrDefault();
        }

        public bool CanChangeType
        {
            get { return SelectedType != null; }
        }
        public async void ChangeType()
        {
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddWarehouseTypeViewModel(SelectedType!));
            if (response.GetValueOrDefault(false))
            {
                using WarehouseContext db = new();
                Types = db.WarehouseTypes.Select(wt => wt.Name).ToList();
            }
        }

        public bool CanAddWarehouse
        {
            get { return !string.IsNullOrWhiteSpace(Address) && !string.IsNullOrWhiteSpace(Phone); }
        }

        public async void AddWarehouse()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();

            Warehouse warehouse = new()
            {
                Address = Address,
                Phone = Phone,
                WarehouseTypeNavigation = db.WarehouseTypes.Where(wt => wt.Name == SelectedType).FirstOrDefault() ?? null,
            };

            if (oldWarehouse != null)
            {
                var foundWarehouse = db.Warehouses.Where(w => w.Id == oldWarehouse.Id).FirstOrDefault();
                if (foundWarehouse != null)
                {
                    foundWarehouse.Address = Address;
                    foundWarehouse.Phone = Phone;
                    foundWarehouse.WarehouseTypeNavigation = db.WarehouseTypes.Where(wt => wt.Name == SelectedType).FirstOrDefault() ?? null;
                }
            }
            else
            {
                db.Warehouses.Add(warehouse);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (Address.Length < 5)
            {
                MessageBox.Show("Длина адреса не может быть короче 5 символов");
                return false;
            }

            if (Phone.Length < 11)
            {
                MessageBox.Show("Номер телефона должен состоять минимум из 11 цифр");
                return false;
            }

            using WarehouseContext db = new();
            var warehouse = db.Warehouses.Where(w => w.Address == Address && w.Phone == Phone).FirstOrDefault();
            if (warehouse != null)
            {
                // changing only Type
                if (oldWarehouse!= null && SelectedType != oldWarehouse.WarehouseType)
                {
                    return true;
                }

                MessageBox.Show("Такой склад уже есть");
                return false;
            }

            return true;
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }
    }
}
