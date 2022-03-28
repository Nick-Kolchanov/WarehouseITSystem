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
    internal class AddMeasurementUnitViewModel : Screen
    {
        public AddMeasurementUnitViewModel() { }
        public AddMeasurementUnitViewModel(string unitName)
        {
            using WarehouseContext db = new();
            oldUnitName = unitName;
            UnitName = unitName;
            UnitShortName = db.MeasurementUnits.Where(m => m.Name == unitName).FirstOrDefault()!.ShortName ?? "";
        }
        private readonly string oldUnitName = "";

        private string unitName = "";
        public string UnitName
        {
            get => unitName;
            set
            {
                unitName = value;
                NotifyOfPropertyChange(() => UnitName);
                NotifyOfPropertyChange(() => CanAddUnit);
            }
        }

        private string unitShortName = "";
        public string UnitShortName
        {
            get => unitShortName;
            set
            {
                unitShortName = value;
                NotifyOfPropertyChange(() => UnitShortName);
                NotifyOfPropertyChange(() => CanAddUnit);
            }
        }

        public async void AddUnit()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();

            if (oldUnitName != "")
            {
                var unitname = db.MeasurementUnits.Where(m => m.Name == oldUnitName).FirstOrDefault()!;
                unitname.Name = UnitName;
                unitname.ShortName = UnitShortName;
            }
            else
            {
                db.MeasurementUnits.Add(new MeasurementUnit { Name = UnitName, ShortName = UnitShortName });
            }
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        public bool CanAddUnit
        {
            get { return !string.IsNullOrWhiteSpace(UnitName) && !string.IsNullOrWhiteSpace(UnitShortName); }
        }

        private bool Validate()
        {
            if (UnitName.Length < 3)
            {
                MessageBox.Show("Длина наименования единицы измерения не может быть меньше 3");
                return false;
            }

            using WarehouseContext db = new();
            var measurementUnits = db.MeasurementUnits.Where(mu => mu.Name == UnitName).FirstOrDefault();
            if (measurementUnits != null)
            {
                MessageBox.Show("Такая единица измерения уже есть");
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
