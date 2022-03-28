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
    internal class AddInventarizationViewModel : Screen
    {
        public AddInventarizationViewModel() : base() 
        {
            LoadValues();
        }

        public AddInventarizationViewModel(InventarizationToScreen inventarization) : base()
        {
            using WarehouseContext db = new();
            oldInventarization = inventarization;
            StartDate = inventarization.StartDate.ToDateTime(new TimeOnly());
            SelectedWarehouse = db.Warehouses.Where(w => w.Id == inventarization.WarehouseId).FirstOrDefault()!.Address;
            SelectedReason = inventarization.Reason;
            LoadValues();
        }

        private void LoadValues()
        {
            using WarehouseContext db = new();
            Warehouses = db.Warehouses.Select(s => s.Address!.ToString()).ToList();
            Reasons = db.InventarizationReasons.Select(s => s.Name!).ToList();
        }

        private readonly InventarizationToScreen? oldInventarization;

        private DateTime? startDate;
        public DateTime? StartDate
        { 
            get => startDate; 
            set 
            {
                startDate = value; 
                NotifyOfPropertyChange(() => StartDate);
                NotifyOfPropertyChange(() => CanAddInventarization);
            } 
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
                NotifyOfPropertyChange(() => CanAddInventarization);
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
                NotifyOfPropertyChange(() => CanAddInventarization);
            }
        }

        private List<string> reasons = null!;
        public List<string> Reasons
        {
            get => reasons;
            set
            {
                reasons = value;
                NotifyOfPropertyChange(() => Reasons);
                NotifyOfPropertyChange(() => CanAddInventarization);
            }
        }

        private string? selectedReason;
        public string? SelectedReason
        {
            get => selectedReason;
            set
            {
                selectedReason = value;
                NotifyOfPropertyChange(() => SelectedReason);
                NotifyOfPropertyChange(() => CanAddInventarization);
            }
        }  

        public bool CanAddInventarization
        {
            get { return SelectedWarehouse != null && SelectedReason != null && StartDate != null; }
        }

        public async void AddInventarization()
        {
            if (!Validate())
                return;

            using var db = new WarehouseContext();

            Inventarization inventarization = new()
            {
                StartDate = DateOnly.FromDateTime(StartDate!.Value),
                EndDate = EndDate.HasValue ? DateOnly.FromDateTime(EndDate!.Value) : null,
                ReasonId = db.InventarizationReasons.Where(ir => ir.Name == SelectedReason).FirstOrDefault()!.Id,
                WarehouseId = db.Warehouses.Where(w => w.Address == SelectedWarehouse).FirstOrDefault()!.Id,
            };

            if (oldInventarization != null)
            {
                var foundInventarization = db.Inventarizations.Where(u => u.Id == oldInventarization.Id).FirstOrDefault();
                if (foundInventarization != null)
                {
                    foundInventarization.EndDate = EndDate.HasValue ? DateOnly.FromDateTime(EndDate!.Value) : null;
                    foundInventarization.StartDate = DateOnly.FromDateTime(StartDate!.Value);
                    foundInventarization.ReasonId = inventarization.ReasonId;
                    foundInventarization.WarehouseId = inventarization.WarehouseId;
                }
            }
            else
            {
                db.Inventarizations.Add(inventarization);
            }
            
            await db.SaveChangesAsync();
            await TryCloseAsync(true);
        }

        private bool Validate()
        {
            if (EndDate < StartDate)
            {
                MessageBox.Show("Конец инвентаризации не может быть раньше начала");
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
