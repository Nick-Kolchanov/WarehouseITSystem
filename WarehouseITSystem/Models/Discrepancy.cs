using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Discrepancy
    {
        public int? InventarizationId { get; set; }
        public int? ProductId { get; set; }
        public int? TypeId { get; set; }
        public int? StatusId { get; set; }

        public virtual Inventarization? Inventarization { get; set; }
        public virtual Product? Product { get; set; }
        public virtual DiscrepancyStatus? Status { get; set; }
        public virtual DiscrepancyType? Type { get; set; }
    }
}
