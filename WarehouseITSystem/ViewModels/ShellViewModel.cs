using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace WarehouseITSystem.ViewModels
{
    internal class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel() : base()
        {
            Items.Add(new ProductsViewModel { DisplayName = "Товары" });
            Items.Add(new NomenclaturesViewModel { DisplayName = "Номенклатуры" });
            Items.Add(new WarehousesViewModel { DisplayName = "Склады" });
            Items.Add(new CustomersViewModel { DisplayName = "Покупатели" });
            Items.Add(new SuppliersViewModel { DisplayName = "Поставщики" });
            Items.Add(new InventarizationsViewModel { DisplayName = "Инвентаризации" });
            Items.Add(new ExportViewModel { DisplayName = "Отчеты и импорт" });
            Items.Add(new UsersViewModel { DisplayName = "Пользователи системы" });            
        }
    }
}
