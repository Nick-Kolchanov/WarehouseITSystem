using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using WarehouseITSystem.Models;

namespace WarehouseITSystem.ViewModels
{
    internal class ProductsViewModel : Screen
    {
        readonly IWindowManager manager = new WindowManager();
        public ProductsViewModel() : base()
        {
            InitValues();
            LoadValues();
        }

        private void InitValues()
        {
            using WarehouseContext db = new();
            Types = db.NomenclatureTypes.Select(nt => nt.Name).ToList();
            Types.Insert(0, "Все");
            SelectedType = Types[0];

            Warehouses = db.Warehouses.Select(w => w.Id.ToString() + ". " + w.Address).ToList();
            Warehouses.Sort();
            Warehouses.Insert(0, "Все");
            SelectedWarehouse = Warehouses[0];

            Statuses = Enum.GetValues<Product.ProductStatus>().ToList();
            (Statuses[Statuses.BinarySearch(Product.ProductStatus.На_складе)], Statuses[0]) = (Statuses[0], Statuses[Statuses.BinarySearch(Product.ProductStatus.На_складе)]);
            SelectedStatuse = Statuses[0];
        }

        private void LoadValues()
        {
            using WarehouseContext db = new();
            db.Nomenclatures.Load();
            var tmpProducts = db.Products.AsQueryable();
            if (SelectedType != "Все")
                tmpProducts = tmpProducts.Where(p => p.Nomenclature!.Type!.Name == SelectedType);
            
            if (SelectedWarehouse != "Все")
            {
                var tmpWarehouse = SelectedWarehouse.Split('.');
                if (int.TryParse(tmpWarehouse[0], out int warehouseNum))
                    tmpProducts = tmpProducts.Where(p => p.WarehouseId == warehouseNum);
            }

            if (SelectedStatuse != Product.ProductStatus.На_складе)
                tmpProducts = tmpProducts.Where(p => p.Status == SelectedStatuse);                   
            else
                tmpProducts = tmpProducts.Where(p => p.Status != Product.ProductStatus.Списан && p.Status != Product.ProductStatus.Продан);

            Products = new BindableCollection<ProductToScreen>(tmpProducts.GroupBy(p => new
            {
                p.NomenclatureId,
                p.Nomenclature!.Name,
                TypeName = p.Nomenclature!.Type!.Name,
                p.Status,
                p.WarehouseId

            }).Select(u => new ProductToScreen
            {                
                NomenclatureId = u.Key.NomenclatureId,
                Name = u.Key.Name,
                Type = u.Key.TypeName,
                Status = u.Key.Status,
                WarehouseId = u.Key.WarehouseId ?? 0,
                Worth = !db.ProductWorths.Where(pw => pw.NomenclatureId == u.Key.NomenclatureId).Any() ? 0 : db.ProductWorths.Where(w => w.NomenclatureId == u.Key.NomenclatureId).OrderByDescending(w => w.Date).ThenByDescending(w => w.Id).FirstOrDefault()!.Worth,
                Count = u.Count()
            }));
        }

        private void LoadProperties()
        {
            if (SelectedProduct == null)
            {
                Properties.Clear();
                return;
            }
            using WarehouseContext db = new();
            Properties = new BindableCollection<PropertyToScreen>(
                db.NomenclatureProperties.Where(np => np.NomenclatureId == SelectedProduct!.NomenclatureId).Select(np => new PropertyToScreen
                {
                    Name = np.Property == null ? "" : np.Property.Name,
                    Value = np.Value,
                    MeasurementUnit = np.Property == null ? "" : np.Property.MeasurementUnit == null ? "" : np.Property.MeasurementUnit.ShortName
                }));

            CellAddress = db.Products.Where(p => p.NomenclatureId == SelectedProduct.NomenclatureId && p.Status == SelectedProduct.Status).Select(p => p.CellAddress).FirstOrDefault() ?? "";
        }

        private IObservableCollection<ProductToScreen> products = null!;
        public IObservableCollection<ProductToScreen> Products
        {
            get => products;
            set
            {
                products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        private ProductToScreen? selectedProduct;
        public ProductToScreen? SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanChangeStatusProduct);
                NotifyOfPropertyChange(() => CanChangeAddress);
                NotifyOfPropertyChange(() => CanSellProduct);

                if (value != null) LoadProperties();
                NotifyOfPropertyChange(() => Properties);
                NotifyOfPropertyChange(() => CellAddress);
            }
        }

        private IObservableCollection<PropertyToScreen> properties = new BindableCollection<PropertyToScreen>();
        public IObservableCollection<PropertyToScreen> Properties
        {
            get => properties;
            set
            {
                properties = value;
                NotifyOfPropertyChange(() => Properties);
            }
        }

        private IObservableCollection<ProductToScreen> sellings = new BindableCollection<ProductToScreen>();
        public IObservableCollection<ProductToScreen> Sellings
        {
            get => sellings;
            set
            {
                sellings = value;
                NotifyOfPropertyChange(() => Sellings);
            }
        }

        private ProductToScreen? selectedSelling;
        public ProductToScreen? SelectedSelling
        {
            get => selectedSelling;
            set
            {
                selectedSelling = value;
                NotifyOfPropertyChange(() => SelectedSelling);
                NotifyOfPropertyChange(() => CanChangeStatusProduct);
                NotifyOfPropertyChange(() => CanCancelSell);
            }
        }

        public List<string> Types { get; set; } = new List<string>();
        private string selectedType = "";
        public string SelectedType { get => selectedType; set { selectedType = value; NotifyOfPropertyChange(() => SelectedType); LoadValues(); } }

        public List<Product.ProductStatus> Statuses { get; set; } = new List<Product.ProductStatus>();
        private Product.ProductStatus selectedStatuse;
        public Product.ProductStatus SelectedStatuse { get => selectedStatuse; set { selectedStatuse = value; NotifyOfPropertyChange(() => SelectedStatuse); LoadValues(); } }

        public List<string> Warehouses { get; set; } = new List<string>();
        private string selectedWarehouse = "";
        public string SelectedWarehouse { get => selectedWarehouse; set { selectedWarehouse = value; NotifyOfPropertyChange(() => SelectedWarehouse); LoadValues(); } }
        
        private string cellAddress = "";
        public string CellAddress { get => cellAddress; set { cellAddress = value; NotifyOfPropertyChange(() => CellAddress); } }
        
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
            foreach (var nomenclature in Products)
            {
                if (nomenclature.Name!.Contains(FilterText))
                {
                    SelectedProduct = nomenclature;
                    return;
                }
            }

            MessageBox.Show("Товар не найден");
        }

        public void RefreshValues()
        {
            LoadValues();
            LoadProperties();
            Sellings.Clear();
            CellAddress = "";
            NotifyOfPropertyChange(() => CanCheckoutSell);
        }

        public async void AddProduct()
        {
            var response = await manager.ShowDialogAsync(new AddProductViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanSellProduct
        {
            get => SelectedProduct != null;
        }
        public async void SellProduct()
        {            
            if (SelectedProduct!.Status != Product.ProductStatus.На_складе)
            {
                MessageBox.Show("Невозможно продать товар с таким статусом");
                return;
            }
            var vm = new AddProductToSellViewModel(SelectedProduct!, Sellings.Where(s => s.NomenclatureId == SelectedProduct!.NomenclatureId).Count());
            var response = await manager.ShowDialogAsync(vm);
            if (response.GetValueOrDefault(false))
            {
                var selling = Sellings.Where(s => s.NomenclatureId == SelectedProduct!.NomenclatureId).FirstOrDefault();
                if (selling == null)
                {
                    Sellings.Add(new ProductToScreen()
                    {
                        NomenclatureId = SelectedProduct!.NomenclatureId,
                        Name = SelectedProduct!.Name,
                        Type = SelectedProduct!.Type,
                        WarehouseId = SelectedProduct!.WarehouseId,
                        Status= SelectedProduct!.Status,
                        Worth = SelectedProduct!.Worth,
                        Count = int.Parse(vm.ProductCount)
                    });
                }
                else
                {
                    selling.Count += int.Parse(vm.ProductCount);
                }
                NotifyOfPropertyChange(() => CanCheckoutSell);
            }            
        }

        public bool CanCheckoutSell
        {
            get => Sellings.Count > 0;
        }
        public async void CheckoutSell()
        {
            var response = await manager.ShowDialogAsync(new SellingViewModel(Sellings.ToList()));
            if (response.GetValueOrDefault(false))
            {
                RefreshValues();
            }
        }

        public bool CanCancelSell
        {
            get => SelectedSelling != null;
        }
        public void CancelSell()
        {
            Sellings.Remove(SelectedSelling!);
        }

        public bool CanChangeStatusProduct
        {
            get => SelectedProduct != null;
        }
        public async void ChangeStatusProduct()
        {
            var response = await manager.ShowDialogAsync(new ChangeStatusViewModel(SelectedProduct!.Status, SelectedProduct.NomenclatureId));
            if (response.GetValueOrDefault(false))
            {
                RefreshValues();
            }
        }

        public bool CanChangeAddress
        {
            get => SelectedProduct != null && SelectedProduct.WarehouseId != 0;
        }
        public async void ChangeAddress()
        {
            using WarehouseContext db = new();
            var vm = new AddProductToWarehouseViewModel(SelectedProduct!.Name, SelectedProduct!.Type);
            var response = await manager.ShowDialogAsync(vm);
            if (response.GetValueOrDefault(false))
            {
                var product = db.Products.Where(p => p.NomenclatureId == SelectedProduct!.NomenclatureId && 
                SelectedProduct.WarehouseId == p.WarehouseId &&
                SelectedProduct.Status == p.Status).FirstOrDefault();
                product!.Warehouse = db.Warehouses.Where(w => w.Address == vm.NewWarehouse).FirstOrDefault();
                product!.CellAddress = vm.NewCellAddress;
            }

            await db.SaveChangesAsync();
        }
    }
}
