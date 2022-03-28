using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using WarehouseITSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseITSystem.ViewModels
{
    internal class AddNomenclatureViewModel : Screen
    {
        public AddNomenclatureViewModel() : base() 
        {
            LoadTypes();
        }

        public AddNomenclatureViewModel(NomenclatureToScreen nomenclature) : base()
        {
            oldNomenclature = nomenclature;
            NomenclatureName = nomenclature.Name;
            Worth = nomenclature.Worth.ToString();            
            SelectedType = nomenclature.Type;
            LoadTypes();
        }

        private void LoadTypes()
        {
            using WarehouseContext db = new();
            Types = db.NomenclatureTypes.Select(nt => nt.Name).ToList();
        }

        private readonly NomenclatureToScreen? oldNomenclature;

        private string nomenclatureName = "";
        public string NomenclatureName
        {
            get => nomenclatureName;
            set
            {
                nomenclatureName = value;
                NotifyOfPropertyChange(() => NomenclatureName);
                NotifyOfPropertyChange(() => CanAddNomenclature);
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
                NotifyOfPropertyChange(() => CanAddNomenclature);
                NotifyOfPropertyChange(() => CanDeleteType);
                NotifyOfPropertyChange(() => CanChangeType);
            }
        }

        private ICollection<string> types = new List<string>();
        public ICollection<string> Types
        {
            get => types;
            set
            {
                types = value;
                NotifyOfPropertyChange(() => Types);
                NotifyOfPropertyChange(() => CanAddNomenclature);
            }
        }

        private string worth = "";
        public string Worth
        { 
            get => worth; 
            set 
            {
                worth = value; 
                NotifyOfPropertyChange(() => Worth);
                NotifyOfPropertyChange(() => CanAddNomenclature);
            } 
        }

        public async void AddType()
        {
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddNomenclatureTypeViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadTypes();
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
            db.NomenclatureTypes.Remove(db.NomenclatureTypes.Where(p => p.Name == SelectedType!).FirstOrDefault()!);
            await db.SaveChangesAsync();

            LoadTypes();
            SelectedType = Types.FirstOrDefault();
        }

        public bool CanChangeType
        {
            get { return SelectedType != null; }
        }
        public async void ChangeType()
        {
            using WarehouseContext db = new();
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddNomenclatureTypeViewModel(db.NomenclatureTypes.Where(p => p.Name == SelectedType!).FirstOrDefault()!));
            if (response.GetValueOrDefault(false))
            {
                LoadTypes();
            }
        }

        public bool CanAddNomenclature
        {
            get { return !string.IsNullOrWhiteSpace(NomenclatureName) && !string.IsNullOrWhiteSpace(Worth) && SelectedType != null; }
        }

        public async void AddNomenclature()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();

            Nomenclature nomenclature = new()
            {
                Name = NomenclatureName,
                Type = db.NomenclatureTypes.Where(nt => nt.Name == SelectedType).FirstOrDefault()!
            };

            ProductWorth productWorth = new()
            {
                Worth = decimal.Parse(Worth),
                Date = DateOnly.FromDateTime(DateTime.Now),
                Nomenclature = nomenclature
            };

            
            if (oldNomenclature != null)
            {
                var foundNomenclature = db.Nomenclatures.Where(u => u.Id == oldNomenclature.Id).FirstOrDefault();
                if (foundNomenclature != null)
                {
                    foundNomenclature.Name = nomenclature.Name;
                    foundNomenclature.Type = nomenclature.Type;
                    productWorth.Nomenclature = foundNomenclature;
                    db.ProductWorths.Add(productWorth);
                }
            }
            else
            {
                db.Nomenclatures.Add(nomenclature);
                db.ProductWorths.Add(productWorth);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (NomenclatureName.Length < 4)
            {
                MessageBox.Show("Наименование номенклатуры не может быть короче 4 символов");
                return false;
            }

            if (decimal.TryParse(Worth, out decimal numWorth))
            {
                if (numWorth <= 0)
                {
                    MessageBox.Show("Стоимость должа быть положительной");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Стоимость должа быть числом");
                return false;
            }

            using WarehouseContext db = new();
            var nomenclature = db.Nomenclatures.Where(n => n.Name == NomenclatureName && n.Type!.Name == SelectedType).FirstOrDefault();
            if (nomenclature != null)
            {
                // changing only Worth
                if (oldNomenclature != null && decimal.Parse(Worth) != oldNomenclature.Worth)
                {
                    return true;
                }

                MessageBox.Show("Такая номенклатура уже есть");
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
