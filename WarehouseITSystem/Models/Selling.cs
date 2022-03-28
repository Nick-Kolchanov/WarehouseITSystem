using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Selling
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public DateOnly Date { get; set; }
        public int? PersonalDiscount { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
