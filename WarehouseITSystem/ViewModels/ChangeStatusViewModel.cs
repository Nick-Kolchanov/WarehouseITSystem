using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using WarehouseITSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace WarehouseITSystem.ViewModels
{
    internal class ChangeStatusViewModel : Screen
    {
        public ChangeStatusViewModel() : base() { }

        public ChangeStatusViewModel(Product.ProductStatus status, int id) : base()
        {
            nomenclatureId = id;
            oldStatus = status;
            StatusInfo = $"Смена статуса с {status} на:";
            Statuses = new List<Product.ProductStatus>();
            switch (status)
            {
                case Product.ProductStatus.На_приемке:
                    Statuses.Add(Product.ProductStatus.На_складе);
                    Statuses.Add(Product.ProductStatus.Списан);
                    break;
                case Product.ProductStatus.На_складе:
                    Statuses.Add(Product.ProductStatus.Списан);
                    break;
                case Product.ProductStatus.Списан:
                    Statuses.Add(Product.ProductStatus.На_складе);
                    break;
                case Product.ProductStatus.Продан:
                    Statuses.Add(Product.ProductStatus.Возвращен);
                    break;
                case Product.ProductStatus.Возвращен:
                    Statuses.Add(Product.ProductStatus.На_складе);
                    Statuses.Add(Product.ProductStatus.Списан);
                    break;
            }
        }
        readonly IWindowManager manager = new WindowManager();
        private readonly int nomenclatureId;
        private readonly Product.ProductStatus oldStatus;

        private string statusInfo = "";
        public string StatusInfo
        {
            get => statusInfo;
            set
            {
                statusInfo = value;
                NotifyOfPropertyChange(() => StatusInfo);
            }
        }

        private List<Product.ProductStatus> statuses = new();
        public List<Product.ProductStatus> Statuses
        {
            get => statuses;
            set
            {
                statuses = value;
                NotifyOfPropertyChange(() => Statuses);
            }
        }

        private Product.ProductStatus? selectedStatuse;
        public Product.ProductStatus? SelectedStatuse
        {
            get => selectedStatuse;
            set
            {
                selectedStatuse = value;
                NotifyOfPropertyChange(() => SelectedStatuse);
                NotifyOfPropertyChange(() => CanChangeStatus);
            }
        }

        public bool CanChangeStatus
        {
            get { return SelectedStatuse != null; }
        }

        public async void ChangeStatus()
        {
            using var db = new WarehouseContext();
            var product = db.Products.Include(p => p.Nomenclature).Include(p => p.Nomenclature!.Type).Where(p => p.Nomenclature!.Id == nomenclatureId && p.Status == oldStatus).FirstOrDefault()!;
            product.Status = SelectedStatuse!.Value;
            if (oldStatus == Product.ProductStatus.На_приемке && SelectedStatuse == Product.ProductStatus.На_складе ||
                oldStatus == Product.ProductStatus.Списан && SelectedStatuse == Product.ProductStatus.На_складе ||
                oldStatus == Product.ProductStatus.Возвращен && SelectedStatuse == Product.ProductStatus.На_складе)
            {
                var vm = new AddProductToWarehouseViewModel(product.Nomenclature!.Name, product.Nomenclature!.Type!.Name);
                var response = await manager.ShowDialogAsync(vm);
                if (response.GetValueOrDefault(false))
                {
                    product.Warehouse = db.Warehouses.Where(w => w.Address == vm.NewWarehouse).FirstOrDefault();
                    product.CellAddress = vm.NewCellAddress;
                }
            }
            else if (oldStatus == Product.ProductStatus.На_складе && SelectedStatuse == Product.ProductStatus.Списан)
            {
                product.CellAddress = "";
            }
            else if (oldStatus == Product.ProductStatus.Продан && SelectedStatuse == Product.ProductStatus.Возвращен)
            {
                db.ProductSellings.RemoveRange(db.ProductSellings.Where(ps => ps.ProductId == product.Id));
                var sell = db.Sellings;
                db.Sellings.RemoveRange(db.Sellings.Where(s => !db.ProductSellings.Where(ps => ps.SellingId == s.Id).Any()));
            }


            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }
    }
}
