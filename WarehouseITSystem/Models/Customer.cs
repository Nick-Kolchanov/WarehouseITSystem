using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Sellings = new HashSet<Selling>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string Phone { get; set; } = null!;

        public virtual ICollection<Selling> Sellings { get; set; }
    }
}
