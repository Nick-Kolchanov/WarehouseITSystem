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
    internal class AddPropertyViewModel : Screen
    {
        public AddPropertyViewModel() : base() 
        {
            LoadUnits();
        }

        public AddPropertyViewModel(Property property) : base()
        {
            oldProperty = property;
            PropertyName = property.Name;   
            SelectedUnit = property.MeasurementUnit!.Name;
            LoadUnits();
        }

        private void LoadUnits()
        {
            using WarehouseContext db = new();
            Units = db.MeasurementUnits.Select(mu => mu.Name).ToList();
        }

        private readonly Property? oldProperty;

        private string propertyName = "";
        public string PropertyName
        {
            get => propertyName;
            set
            {
                propertyName = value;
                NotifyOfPropertyChange(() => PropertyName);
                NotifyOfPropertyChange(() => CanAddProperty);
            }
        }

        private string? selectedUnit;
        public string? SelectedUnit
        {
            get => selectedUnit;
            set
            {
                selectedUnit = value;
                NotifyOfPropertyChange(() => SelectedUnit);
                NotifyOfPropertyChange(() => CanAddProperty);
                NotifyOfPropertyChange(() => CanDeleteUnit);
                NotifyOfPropertyChange(() => CanChangeUnit);
            }
        }

        private ICollection<string?> units = new List<string?>();
        public ICollection<string?> Units
        {
            get => units;
            set
            {
                units = value;
                NotifyOfPropertyChange(() => Units);
                NotifyOfPropertyChange(() => CanAddProperty);
            }
        }

        public async void AddUnit()
        {
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddMeasurementUnitViewModel()); 
            if (response.GetValueOrDefault(false))
            {
                LoadUnits();
            }
        }

        public bool CanDeleteUnit
        {
            get { return SelectedUnit != null; }
        }
        public async void DeleteUnit()
        {
            using WarehouseContext db = new();
            Units.Remove(SelectedUnit!);
            db.MeasurementUnits.Remove(db.MeasurementUnits.Where(p => p.Name == SelectedUnit!).FirstOrDefault()!);
            await db.SaveChangesAsync();

            LoadUnits();
            SelectedUnit = Units.FirstOrDefault();
        }

        public bool CanChangeUnit
        {
            get { return SelectedUnit != null; }
        }
        public async void  ChangeUnit()
        {
            WindowManager manager = new();
            var response = await manager.ShowDialogAsync(new AddMeasurementUnitViewModel(SelectedUnit!));
            if (response.GetValueOrDefault(false))
            {
                LoadUnits();
            }
        }

        public bool CanAddProperty
        {
            get { return !string.IsNullOrWhiteSpace(PropertyName) && SelectedUnit != null; }
        }

        public async void AddProperty()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();

            Property property = new()
            {
                Name = PropertyName,
                MeasurementUnit = db.MeasurementUnits.Where(mu => mu.Name == SelectedUnit).FirstOrDefault()!
            };
            
            if (oldProperty != null)
            {
                var foundProperty = db.Properties.Where(u => u.Name == oldProperty.Name && (u.MeasurementUnit == null ? "" : u.MeasurementUnit.Name) == oldProperty.MeasurementUnit!.Name).FirstOrDefault();
                if (foundProperty != null)
                {
                    foundProperty.Name = property.Name;
                    foundProperty.MeasurementUnit = property.MeasurementUnit;
                }
            }
            else
            {
                db.Properties.Add(property);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (PropertyName.Length < 4)
            {
                MessageBox.Show("Наименование свойства не может быть короче 4 символов");
                return false;
            }

            using WarehouseContext db = new();
            var property = db.Properties.Where(p => p.Name == PropertyName && (p.MeasurementUnit == null ? "" : p.MeasurementUnit.Name) == SelectedUnit).FirstOrDefault();
            if (property != null)
            {
                MessageBox.Show("Такое свойство уже есть");
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
