using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace WarehouseITSystem.ViewModels
{
    internal class UserShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public UserShellViewModel() : base()
        {
            Items.Add(new ProductsViewModel { DisplayName = "Товары" });         
        }
    }
}
