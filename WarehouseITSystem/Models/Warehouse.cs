using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Warehouse
    {
        public Warehouse()
        {
            Inventarizations = new HashSet<Inventarization>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public int? WarehouseType { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        public virtual WarehouseType? WarehouseTypeNavigation { get; set; }
        public virtual ICollection<Inventarization> Inventarizations { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
