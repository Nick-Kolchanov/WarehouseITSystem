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
    internal class DeliveriesViewModel : Screen
    {
        public DeliveriesViewModel(SupplierToScreen supplier) : base()
        {
            currentSupplier = supplier;
            LoadValues();
            DeliveryInfo = $"Поставки от {currentSupplier.Tin} ({currentSupplier.Name}). Контакт: {(currentSupplier.Email ?? currentSupplier.Name)}";
        }

        public void RefreshValues()
        {
            LoadValues();
            LoadNomenclatureDates();
        }

        private void LoadValues()
        {
            WarehouseContext db = new();

            Dates = db.Deliveries.Where(d => d.SupplierId == currentSupplier.Id).GroupBy(u => u.Date).Select(u => new DateToScreen { Day = u.Key.Day, Month = u.Key.Month, Year = u.Key.Year }).ToList();
        }

        private void LoadNomenclatureDates()
        {
            if (SelectedDate == null)
            {
                NomenclaturesDate.Clear();
                return;
            }
            WarehouseContext db = new();
            db.Nomenclatures.Load();
            db.NomenclatureTypes.Load();
            NomenclaturesDate = new BindableCollection<NomenclatureDateToScreen>(
                db.Deliveries.Include(d => d.Products).Where(d => d.Date.Day == SelectedDate.Day && d.Date.Month == SelectedDate.Month && d.Date.Year == SelectedDate.Year && d.SupplierId == currentSupplier.Id).FirstOrDefault()!.Products.GroupBy(p => new { p.NomenclatureId, p.Nomenclature!.Name, Type = p.Nomenclature!.Type!.Name }).Select(p => new NomenclatureDateToScreen
                {
                    Name = p.Key.Name,
                    Type = p.Key.Type,
                    Worth = !db.ProductWorths.Where(pw => pw.NomenclatureId == p.Key.NomenclatureId).Any() ? 0 : db.ProductWorths.Where(w => w.NomenclatureId == p.Key.NomenclatureId).OrderByDescending(w => w.Date).ThenByDescending(w => w.Id).FirstOrDefault()!.Worth,
                    Count = p.Count()
                }));
        }

        private readonly SupplierToScreen currentSupplier;

        private string deliveryInfo = "";
        public string DeliveryInfo
        {
            get => deliveryInfo;
            set
            {
                deliveryInfo = value;
                NotifyOfPropertyChange(() => DeliveryInfo);
            }
        }

        private List<DateToScreen> dates = null!;
        public List<DateToScreen> Dates
        {
            get => dates;
            set
            {
                dates = value;
                NotifyOfPropertyChange(() => Dates);
            }
        }

        private DateToScreen? selectedDate;
        public DateToScreen? SelectedDate
        {
            get => selectedDate;
            set
            {
                selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
                LoadNomenclatureDates();
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
