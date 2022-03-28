using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Delivery
    {
        public Delivery()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public DateOnly Date { get; set; }

        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
