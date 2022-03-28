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
    internal class InventarizationsViewModel : Screen
    {
        readonly IWindowManager manager = new WindowManager();
        public InventarizationsViewModel() : base()
        {
            LoadValues();
        }

        public void RefreshValues()
        {
            LoadValues();
            LoadDiscrepancies();
        }

        private void LoadValues()
        {
            WarehouseContext db = new();
            db.Nomenclatures.Load();
            db.NomenclatureTypes.Load();
            db.ProductWorths.Load();
            Inventarizations = new BindableCollection<InventarizationToScreen>(
                db.Inventarizations.Select(u => new InventarizationToScreen
                {
                    Id = u.Id,
                    StartDate = u.StartDate,
                    EndDate = u.EndDate,
                    WarehouseId = u.WarehouseId,
                    Reason = u.Reason!.Name
                }));
        }

        private void LoadDiscrepancies()
        {
            if (SelectedInventarization == null)
            {
                Discrepancy.Clear();
                return;
            }
            WarehouseContext db = new();
            db.Inventarizations.Load();
            db.Discrepancies.Load();
            Discrepancy = new BindableCollection<DiscrepancyToScreen>(
                db.Discrepancies.Where(np => np.InventarizationId == SelectedInventarization!.Id).Select(np => new DiscrepancyToScreen
                {
                    InventarizationId = np.InventarizationId,
                    ProductId = np.ProductId,
                    ProductName = np.Product!.Nomenclature!.Name,
                    Status = np.Status!.Name,
                    Type = np.Type!.Name
                }));
        }

        private IObservableCollection<InventarizationToScreen> inventarizations = null!;
        public IObservableCollection<InventarizationToScreen> Inventarizations
        {
            get => inventarizations;
            set
            {
                inventarizations = value;
                NotifyOfPropertyChange(() => Inventarizations);
            }
        }

        private InventarizationToScreen? selectedInventarization;
        public InventarizationToScreen? SelectedInventarization
        {
            get => selectedInventarization;
            set
            {
                selectedInventarization = value;
                NotifyOfPropertyChange(() => SelectedInventarization);
                NotifyOfPropertyChange(() => CanDeleteInventarization);
                NotifyOfPropertyChange(() => CanChangeInventarization);
                NotifyOfPropertyChange(() => CanAddDiscrepancy);
                if (value != null) LoadDiscrepancies();
                NotifyOfPropertyChange(() => Discrepancy);                
            }
        }

        private IObservableCollection<DiscrepancyToScreen> discrepancy = new BindableCollection<DiscrepancyToScreen>();
        public IObservableCollection<DiscrepancyToScreen> Discrepancy
        {
            get => discrepancy;
            set
            {
                discrepancy = value;
                NotifyOfPropertyChange(() => Discrepancy);
            }
        }

        private DiscrepancyToScreen? selectedDiscrepancy;
        public DiscrepancyToScreen? SelectedDiscrepancy
        {
            get => selectedDiscrepancy;
            set
            {
                selectedDiscrepancy = value;
                NotifyOfPropertyChange(() => SelectedDiscrepancy);
                NotifyOfPropertyChange(() => CanDeleteDiscrepancy);
                NotifyOfPropertyChange(() => CanChangeDiscrepancy);
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
            if (int.TryParse(FilterText, out int _))
            {
                foreach (var inventarization in Inventarizations)
                {
                    if (inventarization.StartDate.ToString().Contains(FilterText) || (inventarization.EndDate.HasValue && inventarization.EndDate.ToString().Contains(FilterText)))
                    {
                        SelectedInventarization = inventarization;
                        return;
                    }
                }
            }

            MessageBox.Show("Номенклатура не найдена");
        }

        public async void AddInventarization()
        {
            var response = await manager.ShowDialogAsync(new AddInventarizationViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanChangeInventarization
        {
            get => SelectedInventarization != null;
        }
        public async void ChangeInventarization()
        {
            var response = await manager.ShowDialogAsync(new AddInventarizationViewModel(SelectedInventarization!));
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanDeleteInventarization
        {
            get => SelectedInventarization != null;
        }
        public async void DeleteInventarization()
        {
            using WarehouseContext db = new();

            var foundInventarization = db.Inventarizations.Where(x => x.Id == SelectedInventarization!.Id).FirstOrDefault();

            if (foundInventarization != null)
            {
                Inventarizations.Remove(SelectedInventarization!);
                db.Inventarizations.Remove(foundInventarization);
                await db.SaveChangesAsync();
            }
            LoadDiscrepancies();
        }

        public bool CanAddDiscrepancy
        {
            get => SelectedInventarization != null;
        }
        public async void AddDiscrepancy()
        {
            /*var response = await manager.ShowDialogAsync(new AddNomenclaturePropertyViewModel(SelectedInventarization!));
            if (response.GetValueOrDefault(false))
            {
                LoadDiscrepancies();
            }*/
        }

        public bool CanChangeDiscrepancy
        {
            get => SelectedDiscrepancy != null;
        }
        public async void ChangeDiscrepancy()
        {
            /*var response = await manager.ShowDialogAsync(new AddNomenclaturePropertyViewModel(SelectedInventarization!, SelectedDiscrepancy!));
            if (response.GetValueOrDefault(false))
            {
                LoadDiscrepancies();
            }*/
        }

        public bool CanDeleteDiscrepancy
        {
            get => SelectedDiscrepancy != null;
        }
        public async void DeleteDiscrepancy()
        {
            using WarehouseContext db = new();

            var foundDiscrepancy = db.Discrepancies.Where(np => np.InventarizationId == SelectedDiscrepancy!.InventarizationId && np.ProductId == SelectedDiscrepancy!.ProductId).FirstOrDefault();

            if (foundDiscrepancy != null)
            {
                Discrepancy.Remove(SelectedDiscrepancy!);
                db.Discrepancies.Remove(foundDiscrepancy);
                await db.SaveChangesAsync();
            }
        }
    }
}
