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
    internal class AddWarehouseTypeViewModel : Screen
    {
        public AddWarehouseTypeViewModel() { }

        public AddWarehouseTypeViewModel(string typeName) 
        {
            oldTypeName = typeName;
            TypeName = typeName;
        }

        private readonly string oldTypeName = "";

        private string typeName = "";
        public string TypeName
        {
            get => typeName;
            set
            {
                typeName = value;
                NotifyOfPropertyChange(() => TypeName);
                NotifyOfPropertyChange(() => CanAddType);
            }
        }

        public async void AddType()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();

            if (oldTypeName != "")
            {
                var type = db.WarehouseTypes.Where(wt => wt.Name == oldTypeName).FirstOrDefault();
                type!.Name = TypeName;
            }
            else
            {
                db.WarehouseTypes.Add(new WarehouseType { Name = TypeName });
            }
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        public bool CanAddType
        {
            get { return !string.IsNullOrWhiteSpace(TypeName); }
        }

        private bool Validate()
        {
            if (TypeName.Length < 3)
            {
                MessageBox.Show("Наименование типа не может быть короче 3 символов");
                return false;
            }

            using WarehouseContext db = new();
            var warehouseType = db.WarehouseTypes.Where(n => n.Name == TypeName).FirstOrDefault();
            if (warehouseType != null)
            {
                MessageBox.Show("Такой тип уже есть");
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
