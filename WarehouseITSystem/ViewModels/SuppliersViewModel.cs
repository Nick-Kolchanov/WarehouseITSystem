using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using WarehouseITSystem.Models;

namespace WarehouseITSystem.ViewModels
{
    internal class SuppliersViewModel : Screen
    {
        readonly IWindowManager manager = new WindowManager();
        public SuppliersViewModel() : base()
        {
            LoadValues();
        }

        public void RefreshValues()
        {
            LoadValues();
        }

        private void LoadValues()
        {
            WarehouseContext db = new();
            db.Suppliers.Load();
            Suppliers = new BindableCollection<SupplierToScreen>(db.Suppliers.Select(u => new SupplierToScreen { 
                Id = u.Id, 
                Tin = u.Tin, 
                Name = u.Name, 
                Phone = u.Phone, 
                Email = u.Email, 
                DeliveriesCount = u.Deliveries.Count}));
        }

        private IObservableCollection<SupplierToScreen> suppliers = null!;
        public IObservableCollection<SupplierToScreen> Suppliers
        {
            get => suppliers;
            set
            {
                suppliers = value;
                NotifyOfPropertyChange(() => Suppliers);
            }
        }

        private SupplierToScreen? selectedSupplier;
        public SupplierToScreen? SelectedSupplier
        {
            get => selectedSupplier;
            set
            {
                selectedSupplier = value;
                NotifyOfPropertyChange(() => SelectedSupplier);
                NotifyOfPropertyChange(() => CanDeleteSupplier);
                NotifyOfPropertyChange(() => CanChangeSupplier);
                NotifyOfPropertyChange(() => CanCheckDeliveries);                
            }
        }

        private string filterText = string.Empty;
        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                NotifyOfPropertyChange(() => FilterText);
                NotifyOfPropertyChange(() => CanSearchFilter);
            }
        }

        public bool CanSearchFilter
        {
            get => !string.IsNullOrWhiteSpace(FilterText);
        }

        public void SearchFilter()
        {
            if (int.TryParse(FilterText, out int searchId))
            {
                foreach (var supplier in Suppliers)
                {
                    if (supplier.Id == searchId)
                    {
                        SelectedSupplier = supplier;
                        return;
                    }
                }

                foreach (var supplier in Suppliers)
                {
                    if (supplier.Tin == searchId)
                    {
                        SelectedSupplier = supplier;
                        return;
                    }
                }
            }
            else
            {
                foreach (var supplier in Suppliers)
                {
                    if (supplier.Name != null && supplier.Name.Contains(FilterText))
                    {
                        SelectedSupplier = supplier;
                        return;
                    }
                }
            }

            MessageBox.Show("Поставщик не найден");
        }

        public async void AddSupplier()
        {
            var response = await manager.ShowDialogAsync(new AddSupplierViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanChangeSupplier
        {
            get => SelectedSupplier != null;
        }
        public async void ChangeSupplier()
        {
            var response = await manager.ShowDialogAsync(new AddSupplierViewModel(SelectedSupplier!));
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanDeleteSupplier
        {
            get => SelectedSupplier != null;
        }
        public async void DeleteSupplier()
        {
            using WarehouseContext db = new();

            var foundSupplier = db.Suppliers.Where(x => x.Id == SelectedSupplier!.Id).FirstOrDefault();

            if (foundSupplier != null)
            {
                Suppliers.Remove(SelectedSupplier!);
                db.Suppliers.Remove(foundSupplier);
                await db.SaveChangesAsync();
            }
        }

        public bool CanCheckDeliveries
        {
            get => SelectedSupplier != null;
        }
        public async void CheckDeliveries()
        {
            await manager.ShowDialogAsync(new DeliveriesViewModel(SelectedSupplier!));
        }
    }
}
