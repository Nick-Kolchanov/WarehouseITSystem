using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class WarehouseType
    {
        public WarehouseType()
        {
            Warehouses = new HashSet<Warehouse>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
