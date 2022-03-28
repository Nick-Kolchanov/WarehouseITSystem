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
    internal class AddProductToWarehouseViewModel : Screen
    {
        public AddProductToWarehouseViewModel(string name, string type)
        {
            using WarehouseContext db = new();
            ProductName = $"Добавление {name} ({type})";
            Warehouses = db.Warehouses.Select(s => s.Address!.ToString()).ToList();
        }

        private string productName = "";
        public string ProductName
        {
            get => productName;
            set
            {
                productName = value;
                NotifyOfPropertyChange(() => ProductName);
            }
        }

        private string newCellAddress = "";
        public string NewCellAddress
        {
            get => newCellAddress;
            set
            {
                newCellAddress = value;
                NotifyOfPropertyChange(() => NewCellAddress);
                NotifyOfPropertyChange(() => CanAddProductToWarehouse);
            }
        }

        private string newWarehouse = "";
        public string NewWarehouse
        {
            get => newWarehouse;
            set
            {
                newWarehouse = value;
            }
        }

        private List<string> warehouses = null!;
        public List<string> Warehouses
        {
            get => warehouses;
            set
            {
                warehouses = value;
                NotifyOfPropertyChange(() => Warehouses);
            }
        }

        private string? selectedWarehouse;
        public string? SelectedWarehouse
        {
            get => selectedWarehouse;
            set
            {
                selectedWarehouse = value;
                NotifyOfPropertyChange(() => SelectedWarehouse);
                NotifyOfPropertyChange(() => CanAddProductToWarehouse);
            }
        }

        public async void AddProductToWarehouse()
        {
            if (!Validate())
                return;

            NewWarehouse = SelectedWarehouse!;

            await TryCloseAsync(true);
        }

        public bool CanAddProductToWarehouse
        {
            get { return !string.IsNullOrWhiteSpace(NewCellAddress) && SelectedWarehouse != null; }
        }

        private bool Validate()
        {
            using WarehouseContext db = new();
            if (db.Products.Where(p => (p.Warehouse == null ? "" : p.Warehouse.Address) == SelectedWarehouse && p.CellAddress == NewCellAddress).Any())
            {
                MessageBox.Show("Эта ячейка уже занята");
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
