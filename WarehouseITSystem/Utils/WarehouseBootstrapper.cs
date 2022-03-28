using System.Windows;
using Caliburn.Micro;
using WarehouseITSystem.ViewModels;

namespace WarehouseITSystem.Utils
{
    internal class WarehouseBootstrapper : BootstrapperBase
    {
        public WarehouseBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
