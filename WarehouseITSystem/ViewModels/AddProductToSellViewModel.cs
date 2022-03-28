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
    internal class AddProductToSellViewModel : Screen
    {
        public AddProductToSellViewModel(ProductToScreen product, int count)
        {
            ProductName = $"Добавление в корзину {product.Name} ({product.Type})";
            ProductCount = "1";
            curProduct = product;
            curCount = count;
        }

        private readonly ProductToScreen curProduct;
        private readonly int curCount;

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

        private string productCount = "";
        public string ProductCount
        {
            get => productCount;
            set
            {
                productCount = value;
                NotifyOfPropertyChange(() => ProductCount);
                NotifyOfPropertyChange(() => CanAddProductToSell);
            }
        }

        public async void AddProductToSell()
        {
            if (!Validate())
                return;

            await TryCloseAsync(true);
        }

        public bool CanAddProductToSell
        {
            get { return !string.IsNullOrWhiteSpace(ProductCount); }
        }

        private bool Validate()
        {
            if (!int.TryParse(ProductCount, out int ProductCountNum))
            {
                MessageBox.Show("Количество продуктов должно быть числом");
                return false;
            }
            else
            {
                if (ProductCountNum <= 0)
                {
                    MessageBox.Show("Количество продуктов должно быть положительным числом");
                    return false;
                }
            }

            using WarehouseContext db = new();
            var allCount = db.Products.Where(p => p.NomenclatureId == curProduct.NomenclatureId && p.Status == Product.ProductStatus.На_складе).Count();
            if (allCount < ProductCountNum + curCount)
            {
                MessageBox.Show($"Невозможно добавить {ProductCountNum} продукта(ов) в корзину - их всего осталось {allCount - curCount} шт.");
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
