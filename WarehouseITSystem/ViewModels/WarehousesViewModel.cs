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
    internal class WarehousesViewModel : Screen
    {
        readonly IWindowManager manager = new WindowManager();
        public WarehousesViewModel() : base()
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
            db.Warehouses.Load();
            db.WarehouseTypes.Load();
            Warehouses = new BindableCollection<WarehouseToScreen>(
                db.Warehouses.Select(u => new WarehouseToScreen
                { 
                    Id = u.Id,
                    WarehouseType = u.WarehouseTypeNavigation == null ? "" : u.WarehouseTypeNavigation.Name,
                    Address = u.Address,
                    Phone = u.Phone,
                    ProductCount = u.Products.Count
                }));
        }   

        private IObservableCollection<WarehouseToScreen> warehouses = null!;
        public IObservableCollection<WarehouseToScreen> Warehouses
        {
            get => warehouses;
            set
            {
                warehouses = value;
                NotifyOfPropertyChange(() => Warehouses);
            }
        }

        private WarehouseToScreen? selectedWarehouse;
        public WarehouseToScreen? SelectedWarehouse
        {
            get => selectedWarehouse;
            set
            {
                selectedWarehouse = value;
                NotifyOfPropertyChange(() => SelectedWarehouse);
                NotifyOfPropertyChange(() => CanDeleteWarehouse);
                NotifyOfPropertyChange(() => CanChangeWarehouse);            
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
                foreach (var warehouse in Warehouses)
                {
                    if (warehouse.Id == searchId)
                    {
                        SelectedWarehouse = warehouse;
                        return;
                    }
                }

                foreach (var warehouse in Warehouses)
                {
                    if (warehouse.Address!.Contains(FilterText))
                    {
                        SelectedWarehouse = warehouse;
                        return;
                    }
                }
            }
            else
            {
                foreach (var warehouse in Warehouses)
                {
                    if (warehouse.Address!.Contains(FilterText))
                    {
                        SelectedWarehouse = warehouse;
                        return;
                    }
                }
            }

            MessageBox.Show("Склад не найден");
        }

        public async void AddWarehouse()
        {
            var response = await manager.ShowDialogAsync(new AddWarehouseViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanChangeWarehouse
        {
            get => SelectedWarehouse != null;
        }
        public async void ChangeWarehouse()
        {
            var response = await manager.ShowDialogAsync(new AddWarehouseViewModel(SelectedWarehouse!));
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanDeleteWarehouse
        {
            get => SelectedWarehouse != null;
        }
        public async void DeleteWarehouse()
        {
            using WarehouseContext db = new();

            var foundWarehouse = db.Warehouses.Where(x => x.Id == SelectedWarehouse!.Id).FirstOrDefault();

            if (foundWarehouse != null)
            {
                Warehouses.Remove(SelectedWarehouse!);
                db.Warehouses.Remove(foundWarehouse);
                await db.SaveChangesAsync();
            }
        }  
    }
}
