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
    internal class AddCustomerViewModel : Screen
    {
        public AddCustomerViewModel() : base() { }

        public AddCustomerViewModel(CustomerToScreen customer) : base()
        {
            oldCustomer = customer;
            CustomerName = customer.Name ?? "-";
            Phone = customer.Phone;
        }

        private readonly CustomerToScreen? oldCustomer;

        private string customerName = "";
        public string CustomerName
        {
            get => customerName;
            set
            {
                customerName = value;
                NotifyOfPropertyChange(() => CustomerName);
                NotifyOfPropertyChange(() => CanAddCustomer);
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
                NotifyOfPropertyChange(() => CanAddCustomer);
            }
        }

        public bool CanAddCustomer
        {
            get { return !string.IsNullOrWhiteSpace(Phone); }
        }

        public async void AddCustomer()
        {
            if (!Validate())
                return;

            Customer customer = new()
            {
                Name = CustomerName,
                Phone = Phone
            };

            using var db = new WarehouseContext();
            if (oldCustomer != null)
            {
                var foundCustomer = db.Customers.Where(u => u.Id == oldCustomer.Id).FirstOrDefault();
                if (foundCustomer != null)
                {
                    foundCustomer.Name = customer.Name;
                    foundCustomer.Phone = customer.Phone;
                }
            }
            else
            {
                db.Customers.Add(customer);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (CustomerName.Length < 4 && CustomerName.Length != 0)
            {
                MessageBox.Show("Длина имени не может быть меньше 4 знаков");
                return false;
            }

            if (Phone.Length < 11)
            {
                MessageBox.Show("Номер телефона должен состоять минимум из 11 знаков");
                return false;
            }

            using WarehouseContext db = new();
            var user = db.Customers.Where(c => c.Name == CustomerName && c.Phone == Phone).FirstOrDefault();
            if (user != null)
            {
                MessageBox.Show("Такой покупатель уже есть");
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
