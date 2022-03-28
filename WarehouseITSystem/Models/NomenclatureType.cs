using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class NomenclatureType
    {
        public NomenclatureType()
        {
            Nomenclatures = new HashSet<Nomenclature>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<Nomenclature>? Nomenclatures { get; set; }
    }
}
