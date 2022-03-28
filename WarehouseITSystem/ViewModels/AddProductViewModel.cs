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
    internal class AddProductViewModel : Screen
    {
        public AddProductViewModel() : base()
        { 
            LoadValues();
        }

        private void LoadValues()
        {
            WarehouseContext db = new();
            Nomenclatures = new BindableCollection<NomenclatureToScreen>(
                db.Nomenclatures.Select(u => new NomenclatureToScreen 
                { 
                    Id = u.Id, 
                    Name = u.Name, 
                    Type = u.Type == null ? "" : u.Type.Name, 
                    Worth = !u.ProductWorths.Where(w => w.NomenclatureId == u.Id).Any() ? 0 : u.ProductWorths.Where(w => w.NomenclatureId == u.Id).OrderByDescending(w => w.Date).ThenByDescending(w => w.Id).FirstOrDefault()!.Worth
                }));

            Suppliers = db.Suppliers.Select(s => s.Tin.ToString() + " / " + s.Name).ToList();
            Warehouses = db.Warehouses.Select(s => s.Address!.ToString()).ToList();
        }

        private IObservableCollection<NomenclatureToScreen> nomenclatures = null!;
        public IObservableCollection<NomenclatureToScreen> Nomenclatures
        {
            get => nomenclatures;
            set
            {
                nomenclatures = value;
                NotifyOfPropertyChange(() => Nomenclatures);
            }
        }

        private NomenclatureToScreen? selectedNomenclature;
        public NomenclatureToScreen? SelectedNomenclature
        {
            get => selectedNomenclature;
            set
            {
                selectedNomenclature = value;
                NotifyOfPropertyChange(() => SelectedNomenclature);
                NotifyOfPropertyChange(() => CanAddProduct);
            }
        }

        private List<string> suppliers = null!;
        public List<string> Suppliers
        {
            get => suppliers;
            set
            {
                suppliers = value;
                NotifyOfPropertyChange(() => Suppliers);
            }
        }

        private string? selectedSupplier;
        public string? SelectedSupplier
        {
            get => selectedSupplier;
            set
            {
                selectedSupplier = value;
                NotifyOfPropertyChange(() => SelectedSupplier);
                NotifyOfPropertyChange(() => CanAddProduct);
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
                NotifyOfPropertyChange(() => CanAddProduct);
            }
        }

        private string deliveryDate = string.Empty;
        public string DeliveryDate
        {
            get => deliveryDate;
            set
            {
                deliveryDate = value;
                NotifyOfPropertyChange(() => DeliveryDate);
                NotifyOfPropertyChange(() => CanAddProduct);
            }
        }

        private string productCount = string.Empty;
        public string ProductCount
        {
            get => productCount;
            set
            {
                productCount = value;
                NotifyOfPropertyChange(() => ProductCount);
                NotifyOfPropertyChange(() => CanAddProduct);
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
                foreach (var nomenclature in Nomenclatures)
                {
                    if (nomenclature.Id == searchId)
                    {
                        SelectedNomenclature = nomenclature;
                        return;
                    }
                }

                foreach (var nomenclature in Nomenclatures)
                {
                    if (nomenclature.Name!.Contains(FilterText))
                    {
                        SelectedNomenclature = nomenclature;
                        return;
                    }
                }
            }
            else
            {
                foreach (var nomenclature in Nomenclatures)
                {
                    if (nomenclature.Name!.Contains(FilterText))
                    {
                        SelectedNomenclature = nomenclature;
                        return;
                    }
                }
            }

            MessageBox.Show("Наименование не найдено");
        }

        public bool CanAddProduct
        {
            get => SelectedNomenclature != null &&
                !string.IsNullOrWhiteSpace(SelectedSupplier) &&
                !string.IsNullOrWhiteSpace(SelectedWarehouse) && 
                !string.IsNullOrWhiteSpace(ProductCount) &&
                !string.IsNullOrWhiteSpace(DeliveryDate);
        }
        public async void AddProduct()
        {
            if (!Validate())
                return;

            using WarehouseContext db = new();

            var deliveries = db.Deliveries.Where(d => d.SupplierId == db.Suppliers.Where(s => s.Tin == long.Parse(SelectedSupplier!)).Select(s => s.Id).FirstOrDefault());
            var deliveryId = 0;
            if (deliveries.Select(d => d.Date).Contains(DateOnly.FromDateTime(DateTime.Parse(DeliveryDate))))
            {
                deliveryId = deliveries.Where(d => d.Date == DateOnly.FromDateTime(DateTime.Parse(DeliveryDate))).Select(d => d.Id).FirstOrDefault();
            }
            else
            {
                Delivery newDelivery = new()
                {
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    SupplierId = db.Suppliers.Where(s => s.Tin == long.Parse(SelectedSupplier!)).Select(s => s.Id).FirstOrDefault()
                };
                db.Deliveries.Add(newDelivery);
                await db.SaveChangesAsync();

                deliveryId = db.Deliveries.Where(d => d.SupplierId == db.Suppliers.Where(s => s.Tin == long.Parse(SelectedSupplier!)).Select(s => s.Id).FirstOrDefault() && d.Date == DateOnly.FromDateTime(DateTime.Now)).Select(d => d.Id).FirstOrDefault();
            }

            for (int i = 0; i < int.Parse(ProductCount); i++)
            {
                Product newProduct = new()
                {
                    NomenclatureId = SelectedNomenclature!.Id,
                    DeliveryId = deliveryId,
                    WarehouseId = db.Warehouses.Where(w => w.Address == SelectedWarehouse).Select(w => w.Id).FirstOrDefault(),
                    Status = Product.ProductStatus.На_приемке
                };

                db.Products.Add(newProduct);
            }

            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (int.TryParse(ProductCount, out int productNum))
            {
                if (productNum <= 0)
                {
                    MessageBox.Show("Количество должно быть положительно");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Количество должно быть числом");
                return false;
            }

            if (!DateTime.TryParse(DeliveryDate, out _))
            {
                MessageBox.Show("Неверный формат даты");
                return false;
            }

            return true;
        }

        public async void Cancel()
        {
            await TryCloseAsync(false);
        }
    }
}
