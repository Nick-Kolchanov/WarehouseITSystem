using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using WarehouseITSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseITSystem.ViewModels
{
    internal class SellingViewModel : Screen
    {
        public SellingViewModel(List<ProductToScreen> products)
        {
            productList = products;
            Nomenclatures = productList.Select(p => p.Name).ToList();
            Summary = $"Итого: {productList.Sum(p => p.Worth)}р.";
        }

        private readonly List<ProductToScreen> productList;

        private string phone = "";
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                NotifyOfPropertyChange(() => Phone);
                FindCustomer();
            }
        }

        private Customer? currentCustomer;
        private void FindCustomer()
        {
            using WarehouseContext db = new();
            currentCustomer = db.Customers.Include(c => c.Sellings).Where(c => c.Phone == Phone).FirstOrDefault();
            if (currentCustomer != null)
            {
                var customerName = (currentCustomer.Name != null && currentCustomer.Name != "") ? $" - {currentCustomer.Name}" : "";
                var customerSellingsId = currentCustomer.Sellings.Select(s => s.Id).ToList();
                var customerProductSellings = db.ProductSellings.Where(ps => customerSellingsId.Contains(ps.SellingId!.Value));
                var customerProductWorth = customerProductSellings.Select(ps => new { Worth = ps.Product!.Nomenclature!.ProductWorths.Where(pw => pw.Date <= ps.Selling!.Date).OrderByDescending(pw => pw.Date).FirstOrDefault()!.Worth * (1 - ps.Selling!.PersonalDiscount/100) }).Sum(pw => pw.Worth);
                customerProductWorth = Math.Round(customerProductWorth == null ? 0 : customerProductWorth.Value);
                CustomerInfo = $"Найден покупатель{customerName}. \n Сумма стоимости его покупок - {customerProductWorth}р.";
            }
            else
            {
                CustomerInfo = $"Покупатель не найден";
            }
        }

        private string customerInfo = "";
        public string CustomerInfo
        {
            get => customerInfo;
            set
            {
                customerInfo = value;
                NotifyOfPropertyChange(() => CustomerInfo);
            }
        }

        private string personalDiscount = "0";
        public string PersonalDiscount
        {
            get => personalDiscount;
            set
            {
                personalDiscount = value;
                NotifyOfPropertyChange(() => PersonalDiscount);
                if (int.TryParse(PersonalDiscount, out int discountNum) && discountNum >= 0 && discountNum <= 100)
                {                    
                    Summary = $"Итого: {productList.Sum(p => p.Worth) - productList.Sum(p => p.Worth) * discountNum / 100}р.";
                }
            }
        }

        private string summary = "";
        public string Summary
        {
            get => summary;
            set
            {
                summary = value;
                NotifyOfPropertyChange(() => Summary);
            }
        }

        private List<string> nomenclatures = new();
        public List<string> Nomenclatures
        {
            get => nomenclatures;
            set
            {
                nomenclatures = value;
                NotifyOfPropertyChange(() => Nomenclatures);
            }
        }

        public async void ConfirmSelling()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();
            var selling = new Selling() { CustomerId = currentCustomer!.Id, Date = DateOnly.FromDateTime(DateTime.Now), PersonalDiscount = int.Parse(PersonalDiscount) };
            var newSelling = db.Sellings.Add(selling);
            await db.SaveChangesAsync();

            for (int i = 0; i < productList.Count; i++)
            {
                var product = db.Products.Where(p => p.NomenclatureId == productList[i].NomenclatureId && p.Status == Product.ProductStatus.На_складе).FirstOrDefault()!;
                db.ProductSellings.Add(new ProductSelling() { SellingId = newSelling.Entity.Id, ProductId = product.Id });
                product.Status = Product.ProductStatus.Продан;
                product.CellAddress = "";
                product.Warehouse = null;
                product.WarehouseId = null;
            }            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (int.TryParse(PersonalDiscount, out int personalDiscountNum))
            {
                if (personalDiscountNum < 0 || personalDiscountNum > 100)
                {
                    MessageBox.Show("Скидка должна быть числом (в процентах - от 0 до 100)");
                    return false;
                }                
            }
            else
            {
                MessageBox.Show("Скидка должна быть числом (в процентах - от 0 до 100)");
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
