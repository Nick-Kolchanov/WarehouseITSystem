using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Nomenclature
    {
        public Nomenclature()
        {
            ProductWorths = new HashSet<ProductWorth>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int? TypeId { get; set; }

        public virtual NomenclatureType? Type { get; set; }
        public virtual ICollection<ProductWorth> ProductWorths { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
