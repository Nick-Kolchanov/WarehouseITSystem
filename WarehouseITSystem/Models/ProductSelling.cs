using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class ProductSelling
    {
        public int? SellingId { get; set; }
        public int? ProductId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual Selling? Selling { get; set; }
    }
}
