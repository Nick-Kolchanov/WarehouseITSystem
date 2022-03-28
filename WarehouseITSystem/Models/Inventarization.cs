using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Inventarization
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? WarehouseId { get; set; }
        public int? ReasonId { get; set; }

        public virtual InventarizationReason? Reason { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
    }
}
