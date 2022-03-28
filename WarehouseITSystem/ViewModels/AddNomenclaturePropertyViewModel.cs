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
    internal class AddNomenclaturePropertyViewModel : Screen
    {
        public AddNomenclaturePropertyViewModel(NomenclatureToScreen nomenclature) : base() 
        {
            oldNomenclature = nomenclature;
            LoadProperties();
        }

        public AddNomenclaturePropertyViewModel(NomenclatureToScreen nomenclature, PropertyToScreen property) : base()
        {
            oldNomenclature = nomenclature;
            oldProperty = property;
            PropertyValue = property.Value;   
            SelectedProperty = property.Name;
            LoadProperties();
        }

        private void LoadProperties()
        {
            using WarehouseContext db = new();
            Properties = db.Properties.Select(p => p.Name).ToList();
        }

        private readonly NomenclatureToScreen oldNomenclature;
        private readonly PropertyToScreen? oldProperty;

        public string OldNomenclatureName { get => oldNomenclature.Name; }

        private string propertyValue = "";
        public string PropertyValue
        {
            get => propertyValue;
            set
            {
                propertyValue = value;
                NotifyOfPropertyChange(() => PropertyValue);
                NotifyOfPropertyChange(() => CanAddNomenclatureProperty);
            }
        }

        private string? selectedProperty;
        public string? SelectedProperty
        {
            get => selectedProperty;
            set
            {
                selectedProperty = value;
                NotifyOfPropertyChange(() => SelectedProperty);
                NotifyOfPropertyChange(() => CanAddNomenclatureProperty);
                NotifyOfPropertyChange(() => CanDeleteProperty);
                NotifyOfPropertyChange(() => CanChangeProperty);
            }
        }

        private ICollection<string> properties = new List<string>();
        public ICollection<string> Properties
        {
            get => properties;
            set
            {
                properties = value;
                NotifyOfPropertyChange(() => Properties);
                NotifyOfPropertyChange(() => CanAddNomenclatureProperty);
            }
        }

        public async void AddProperty()
        {
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddPropertyViewModel()); 
            if (response.GetValueOrDefault(false))
            {
                LoadProperties();
            }
        }

        public bool CanDeleteProperty
        {
            get { return SelectedProperty != null; }
        }
        public async void DeleteProperty()
        {
            using WarehouseContext db = new();
            Properties.Remove(SelectedProperty!);
            db.Properties.Remove(db.Properties.Where(p => p.Name == SelectedProperty!).FirstOrDefault()!);
            await db.SaveChangesAsync();

            LoadProperties();
            SelectedProperty = Properties.FirstOrDefault();
        }

        public bool CanChangeProperty
        {
            get { return SelectedProperty != null; }
        }
        public async void ChangeProperty()
        {
            using WarehouseContext db = new();
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddPropertyViewModel(db.Properties.Include(p => p.MeasurementUnit).Where(p => p.Name == SelectedProperty!).FirstOrDefault()!));
            if (response.GetValueOrDefault(false))
            {
                LoadProperties();
            }
        }

        public bool CanAddNomenclatureProperty
        {
            get { return !string.IsNullOrWhiteSpace(PropertyValue) && SelectedProperty != null; }
        }

        public async void AddNomenclatureProperty()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();

            NomenclatureProperty nomenclatureProperty = new()
            {
                NomenclatureId = oldNomenclature.Id,
                PropertyId = db.Properties.Where(p => p.Name == SelectedProperty).Select(p => p.Id).FirstOrDefault(),
                Value = PropertyValue
            };
            
            if (oldProperty != null)
            {                
                var foundNomenclatureProperty = db.NomenclatureProperties.Where(np => np.NomenclatureId == oldNomenclature.Id && np.PropertyId == db.Properties.Where(p => p.Name == oldProperty.Name).Select(p => p.Id).FirstOrDefault()).FirstOrDefault();
                if (foundNomenclatureProperty != null)
                {
                    if (oldProperty.Name != SelectedProperty)
                    {
                        db.NomenclatureProperties.Remove(foundNomenclatureProperty);
                        await db.SaveChangesAsync();
                        db.NomenclatureProperties.Add(nomenclatureProperty);
                    }
                    else
                    {
                        foundNomenclatureProperty.Value = nomenclatureProperty.Value;
                    }                    
                }
            }
            else
            {
                db.NomenclatureProperties.Add(nomenclatureProperty);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            using WarehouseContext db = new();

            var nomenclatureProperty = db.NomenclatureProperties.Where(np => np.NomenclatureId == oldNomenclature.Id && (np.Property == null ? "" : np.Property.Name) == SelectedProperty).FirstOrDefault();
            if (nomenclatureProperty != null)
            {
                // Changing only value
                if (oldProperty != null && oldProperty.Name == SelectedProperty)
                {
                    return true;
                }
                MessageBox.Show("У такой номенклатуры уже определено это свойство");
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
