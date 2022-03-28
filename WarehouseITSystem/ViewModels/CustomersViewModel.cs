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
    internal class CustomersViewModel : Screen
    {
        readonly IWindowManager manager = new WindowManager();
        public CustomersViewModel() : base()
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
            db.Customers.Load();
            db.ProductSellings.Load();
            db.Sellings.Load();
            db.ProductWorths.Load();
            db.Nomenclatures.Load();
            db.Products.Load();
            Customers = new BindableCollection<CustomerToScreen>(db.Customers.Select(u => new CustomerToScreen { Id = u.Id, Name = u.Name, Phone = u.Phone, 
                SellingSum = Math.Round(db.ProductSellings.Where(ps => u.Sellings.Contains(ps.Selling!))
                                               .Sum(ps => ps.Product!.Nomenclature!.ProductWorths.Where(pw => pw.Date <= ps.Selling!.Date).OrderByDescending(pw => pw.Date).FirstOrDefault() == null ? 
                                               0 : (ps.Product.Nomenclature.ProductWorths.Where(pw => pw.Date <= ps.Selling!.Date).OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth * (1 - ps.Selling!.PersonalDiscount!.Value/100))))
            }));
        }

        private IObservableCollection<CustomerToScreen> customers = null!;
        public IObservableCollection<CustomerToScreen> Customers
        {
            get => customers;
            set
            {
                customers = value;
                NotifyOfPropertyChange(() => Customers);
            }
        }

        private CustomerToScreen? selectedCustomer;
        public CustomerToScreen? SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
                NotifyOfPropertyChange(() => SelectedCustomer);
                NotifyOfPropertyChange(() => CanDeleteCustomer);
                NotifyOfPropertyChange(() => CanChangeCustomer);
                NotifyOfPropertyChange(() => CanCheckBuyings);                
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
                foreach (var customer in Customers)
                {
                    if (customer.Id == searchId)
                    {
                        SelectedCustomer = customer;
                        return;
                    }
                }
            }
            else
            {
                foreach (var customer in Customers)
                {
                    if (customer.Name != null && customer.Name.Contains(FilterText))
                    {
                        SelectedCustomer = customer;
                        return;
                    }
                }
            }

            MessageBox.Show("Покупатель не найден");
        }

        public async void AddCustomer()
        {
            var response = await manager.ShowDialogAsync(new AddCustomerViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanChangeCustomer
        {
            get => SelectedCustomer != null;
        }
        public async void ChangeCustomer()
        {
            var response = await manager.ShowDialogAsync(new AddCustomerViewModel(SelectedCustomer!));
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanDeleteCustomer
        {
            get => SelectedCustomer != null;
        }
        public async void DeleteCustomer()
        {
            using WarehouseContext db = new();

            var foundCustomer = db.Customers.Where(x => x.Id == SelectedCustomer!.Id).FirstOrDefault();

            if (foundCustomer != null)
            {
                Customers.Remove(SelectedCustomer!);
                db.Customers.Remove(foundCustomer);
                await db.SaveChangesAsync();
            }
        }

        public bool CanCheckBuyings
        {
            get => SelectedCustomer != null;
        }
        public async void CheckBuyings()
        {
            await manager.ShowDialogAsync(new BuyingsViewModel(SelectedCustomer!));
        }
    }
}
