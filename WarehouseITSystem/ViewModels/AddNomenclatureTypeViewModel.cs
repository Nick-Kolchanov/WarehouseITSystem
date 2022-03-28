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
    internal class AddNomenclatureTypeViewModel : Screen
    {
        public AddNomenclatureTypeViewModel() { }

        public AddNomenclatureTypeViewModel(NomenclatureType type) 
        {
            oldType = type;
            TypeName = type.Name;
        }
        private readonly NomenclatureType? oldType;

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
            if (oldType != null)
            {
                var type = db.NomenclatureTypes.Where(nt => nt.Id == oldType.Id).FirstOrDefault();
                type!.Name = TypeName;
            }
            else
            {
                db.NomenclatureTypes.Add(new NomenclatureType { Name = TypeName });
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
                MessageBox.Show("Наименование не может быть короче 3 символов");
                return false;
            }

            using WarehouseContext db = new();
            var nomenclature = db.NomenclatureTypes.Where(n => n.Name == TypeName).FirstOrDefault();
            if (nomenclature != null)
            {
                MessageBox.Show("Такой тип товара уже есть");
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
