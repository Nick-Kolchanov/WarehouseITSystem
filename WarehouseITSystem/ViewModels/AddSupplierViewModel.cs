using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using WarehouseITSystem.Models;

namespace WarehouseITSystem.ViewModels
{
    internal class AddSupplierViewModel : Screen
    {
        public AddSupplierViewModel() : base() { }

        public AddSupplierViewModel(SupplierToScreen supplier) : base()
        {
            oldSupplier = supplier;
            SupplierName = supplier.Name ?? "";
            Tin = supplier.Tin.ToString();
            Phone = supplier.Phone ?? "";
            Email = supplier.Email ?? "";            
        }

        private readonly SupplierToScreen? oldSupplier;

        private string supplierName = "";
        public string SupplierName
        {
            get => supplierName;
            set
            {
                supplierName = value;
                NotifyOfPropertyChange(() => SupplierName);
            }
        }

        private string tin = "";
        public string Tin
        {
            get => tin;
            set
            {
                tin = value;
                NotifyOfPropertyChange(() => Tin);
                NotifyOfPropertyChange(() => CanAddSupplier);
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
                NotifyOfPropertyChange(() => CanAddSupplier);
            } 
        }

        private string email = "";
        public string Email
        {
            get => email;
            set
            {
                email = value;
                NotifyOfPropertyChange(() => Email);
                NotifyOfPropertyChange(() => CanAddSupplier);
            }
        }

        public bool CanAddSupplier
        {
            get { return (!string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(Phone)) && !string.IsNullOrWhiteSpace(Tin); }
        }

        public async void AddSupplier()
        {
            if (!Validate())
                return;

            Supplier supplier = new()
            {
                Name = SupplierName,
                Tin = long.Parse(Tin),
                Phone = Phone,
                Email = Email
            };

            using var db = new WarehouseContext();
            if (oldSupplier != null)
            {
                var foundSupplier = db.Suppliers.Where(u => u.Id == oldSupplier.Id).FirstOrDefault();
                if (foundSupplier != null)
                {
                    foundSupplier.Name = supplier.Name;
                    foundSupplier.Tin = supplier.Tin;
                    foundSupplier.Phone = supplier.Phone;
                    foundSupplier.Email = supplier.Email;
                }
            }
            else
            {
                db.Suppliers.Add(supplier);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (SupplierName.Length < 4)
            {
                MessageBox.Show("Имя поставщика не может быть короче 4 символов");
                return false;
            }

            if (!long.TryParse(Tin, out long tinNum))
            {
                MessageBox.Show("ИНН должно состоять только из цифр");
                return false;
            }
            else if (Tin.Length != 10)
            {
                MessageBox.Show("ИНН должно состоять из 10 цифр");
                return false;
            }

            if (Phone.Length < 11)
            {
                MessageBox.Show("Номер телефона должен состоять минимум из 11 цифр");
                return false;
            }    

            if (Email != "" && !MailAddress.TryCreate(Email, out _))
            {
                MessageBox.Show("Неверный формат e-mail");
                return false;
            }

            using WarehouseContext db = new();
            var supplier = db.Suppliers.Where(s => s.Tin == long.Parse(Tin)).FirstOrDefault();
            if (supplier != null)
            {
                if (oldSupplier!= null && long.Parse(Tin) == oldSupplier.Tin)
                {
                    return true;
                }

                MessageBox.Show("Такой поставщик уже есть");
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
