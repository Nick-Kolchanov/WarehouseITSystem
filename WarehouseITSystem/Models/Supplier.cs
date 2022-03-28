using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Deliveries = new HashSet<Delivery>();
        }

        public int Id { get; set; }
        public long Tin { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
    }
}
