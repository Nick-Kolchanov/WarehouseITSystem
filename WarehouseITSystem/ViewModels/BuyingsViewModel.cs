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
    internal class BuyingsViewModel : Screen
    {
        public BuyingsViewModel(CustomerToScreen customer) : base()
        {
            currentCustomer = customer;
            LoadValues();
            BuyingsInfo = $"Покупки от {currentCustomer.Phone} ({currentCustomer.Name})";
        }

        public void RefreshValues()
        {
            LoadValues();
            LoadNomenclatureBuyings();
        }

        private void LoadValues()
        {
            WarehouseContext db = new();

            Sellings = db.Sellings.Where(s => s.CustomerId == currentCustomer.Id).Select(u => new SellingToScreen { Id = u.Id, Date = u.Date, PersonalDiscount = u.PersonalDiscount}).ToList();
        }

        private void LoadNomenclatureBuyings()
        {
            if (SelectedSelling == null)
            {
                NomenclaturesDate.Clear();
                return;
            }
            WarehouseContext db = new();
            db.Nomenclatures.Load();
            db.NomenclatureTypes.Load();
            NomenclaturesDate = new BindableCollection<NomenclatureDateToScreen>(
                db.ProductSellings.Include(d => d.Selling).Include(d => d.Product).Where(s => s.SellingId == selectedSelling!.Id).GroupBy(p => new { p.Product!.NomenclatureId, p.Product!.Nomenclature!.Name, Type = p.Product!.Nomenclature!.Type!.Name }).Select(p => new NomenclatureDateToScreen
                {
                    Name = p.Key.Name,
                    Type = p.Key.Type,
                    Worth = !db.ProductWorths.Where(pw => pw.NomenclatureId == p.Key.NomenclatureId).Any() ? 0 : db.ProductWorths.Where(w => w.NomenclatureId == p.Key.NomenclatureId).OrderByDescending(w => w.Date).ThenByDescending(w => w.Id).FirstOrDefault()!.Worth,
                    Count = p.Count()
                }));
        }

        private readonly CustomerToScreen currentCustomer;

        private string buyingsInfo = "";
        public string BuyingsInfo
        {
            get => buyingsInfo;
            set
            {
                buyingsInfo = value;
                NotifyOfPropertyChange(() => BuyingsInfo);
            }
        }

        private List<SellingToScreen> sellings = null!;
        public List<SellingToScreen> Sellings
        {
            get => sellings;
            set
            {
                sellings = value;
                NotifyOfPropertyChange(() => Sellings);
            }
        }

        private SellingToScreen? selectedSelling;
        public SellingToScreen? SelectedSelling
        {
            get => selectedSelling;
            set
            {
                selectedSelling = value;
                NotifyOfPropertyChange(() => SelectedSelling);
                LoadNomenclatureBuyings();
                NotifyOfPropertyChange(() => NomenclaturesDate);                
            }
        }

        private IObservableCollection<NomenclatureDateToScreen> nomenclaturesDate = new BindableCollection<NomenclatureDateToScreen>();
        public IObservableCollection<NomenclatureDateToScreen> NomenclaturesDate
        {
            get => nomenclaturesDate;
            set
            {
                nomenclaturesDate = value;
                NotifyOfPropertyChange(() => NomenclaturesDate);
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

        public static void SearchFilter()
        {
            // TODO
            MessageBox.Show("Дата не найдена");
        }

        public async void CloseWindow()
        {
            await TryCloseAsync();
        }
    }
}
