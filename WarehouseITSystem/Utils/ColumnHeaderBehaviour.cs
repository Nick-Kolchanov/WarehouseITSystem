using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarehouseITSystem.Utils
{
    public class ColumnHeaderBehaviour : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += OnGeneratingColumn!;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -= OnGeneratingColumn!;
        }

        private static void OnGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs eventArgs)
        {
            if (eventArgs.PropertyDescriptor is PropertyDescriptor descriptor && descriptor.DisplayName != null && descriptor.DisplayName != "null")
            {
                eventArgs.Column.Header = descriptor.DisplayName;
                if (descriptor.Name.Contains("id") || descriptor.Name.Contains("Id"))
                {
                    eventArgs.Column.Width = 60;
                }
            }
            else
            {
                eventArgs.Cancel = true;
            }
        }
    }
}
