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
    internal class NomenclaturesViewModel : Screen
    {
        readonly IWindowManager manager = new WindowManager();
        public NomenclaturesViewModel() : base()
        {
            LoadValues();
        }

        public void RefreshValues()
        {
            LoadValues();
            LoadProperties();
        }

        private void LoadValues()
        {
            WarehouseContext db = new();
            db.Nomenclatures.Load();
            db.NomenclatureTypes.Load();
            db.ProductWorths.Load();
            Nomenclatures = new BindableCollection<NomenclatureToScreen>(
                db.Nomenclatures.Select(u => new NomenclatureToScreen 
                { 
                    Id = u.Id, 
                    Name = u.Name, 
                    Type = u.Type == null ? "" : u.Type.Name, 
                    Worth = !u.ProductWorths.Where(w => w.NomenclatureId == u.Id).Any() ? 0 : u.ProductWorths.Where(w => w.NomenclatureId == u.Id).OrderByDescending(w => w.Date).ThenByDescending(w => w.Id).FirstOrDefault()!.Worth
                }));
        }

        private void LoadProperties()
        {
            if (SelectedNomenclature == null)
            {
                Properties.Clear();
                return;
            }
            WarehouseContext db = new();
            db.NomenclatureProperties.Load();
            db.Properties.Load();
            Properties = new BindableCollection<PropertyToScreen>(
                db.NomenclatureProperties.Where(np => np.NomenclatureId == SelectedNomenclature!.Id).Select(np => new PropertyToScreen
                {
                    Name = np.Property == null ? "" : np.Property.Name,
                    Value = np.Value,
                    MeasurementUnit = np.Property == null? "" : np.Property.MeasurementUnit == null ? "" : np.Property.MeasurementUnit.Name
                }));
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
                NotifyOfPropertyChange(() => CanDeleteNomenclature);
                NotifyOfPropertyChange(() => CanChangeNomenclature);
                NotifyOfPropertyChange(() => CanAddProperty);
                if (value != null) LoadProperties();
                NotifyOfPropertyChange(() => Properties);                
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

        private PropertyToScreen? selectedProperty;
        public PropertyToScreen? SelectedProperty
        {
            get => selectedProperty;
            set
            {
                selectedProperty = value;
                NotifyOfPropertyChange(() => SelectedProperty);
                NotifyOfPropertyChange(() => CanDeleteProperty);
                NotifyOfPropertyChange(() => CanChangeProperty);
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

            MessageBox.Show("Номенклатура не найдена");
        }

        public async void AddNomenclature()
        {
            var response = await manager.ShowDialogAsync(new AddNomenclatureViewModel());
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanChangeNomenclature
        {
            get => SelectedNomenclature != null;
        }
        public async void ChangeNomenclature()
        {
            var response = await manager.ShowDialogAsync(new AddNomenclatureViewModel(SelectedNomenclature!));
            if (response.GetValueOrDefault(false))
            {
                LoadValues();
            }
        }

        public bool CanDeleteNomenclature
        {
            get => SelectedNomenclature != null;
        }
        public async void DeleteNomenclature()
        {
            using WarehouseContext db = new();

            var foundNomenclature = db.Nomenclatures.Where(x => x.Id == SelectedNomenclature!.Id).FirstOrDefault();

            if (foundNomenclature != null)
            {
                Nomenclatures.Remove(SelectedNomenclature!);
                db.Nomenclatures.Remove(foundNomenclature);
                await db.SaveChangesAsync();
            }
            LoadProperties();
        }

        public bool CanAddProperty
        {
            get => SelectedNomenclature != null;
        }
        public async void AddProperty()
        {
            var response = await manager.ShowDialogAsync(new AddNomenclaturePropertyViewModel(SelectedNomenclature!));
            if (response.GetValueOrDefault(false))
            {
                LoadProperties();
            }
        }

        public bool CanChangeProperty
        {
            get => SelectedProperty != null;
        }
        public async void ChangeProperty()
        {
            var response = await manager.ShowDialogAsync(new AddNomenclaturePropertyViewModel(SelectedNomenclature!, SelectedProperty!));
            if (response.GetValueOrDefault(false))
            {
                LoadProperties();
            }
        }

        public bool CanDeleteProperty
        {
            get => SelectedProperty != null;
        }
        public async void DeleteProperty()
        {
            using WarehouseContext db = new();

            var foundNomenclatureProperty = db.NomenclatureProperties.Where(np => (np.Property == null ? "" : np.Property.Name) == SelectedProperty!.Name && np.NomenclatureId == SelectedNomenclature!.Id).FirstOrDefault();

            if (foundNomenclatureProperty != null)
            {
                Properties.Remove(SelectedProperty!);
                db.NomenclatureProperties.Remove(foundNomenclatureProperty);
                await db.SaveChangesAsync();
            }
        }
    }
}
