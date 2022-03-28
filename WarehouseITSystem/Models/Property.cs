using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Property
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int? MeasurementUnitId { get; set; }

        public virtual MeasurementUnit? MeasurementUnit { get; set; }
    }
}
