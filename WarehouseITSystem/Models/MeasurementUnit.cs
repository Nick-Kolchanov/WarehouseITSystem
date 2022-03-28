using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class MeasurementUnit
    {
        public MeasurementUnit()
        {
            Properties = new HashSet<Property>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }

        public virtual ICollection<Property> Properties { get; set; }
    }
}
