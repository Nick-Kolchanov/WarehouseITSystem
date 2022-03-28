using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class ProductWorth
    {
        public int Id { get; set; }
        public int? NomenclatureId { get; set; }
        public decimal Worth { get; set; }
        public DateOnly Date { get; set; }

        public virtual Nomenclature? Nomenclature { get; set; }
    }
}
