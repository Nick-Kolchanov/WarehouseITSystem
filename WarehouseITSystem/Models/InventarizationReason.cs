using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class InventarizationReason
    {
        public InventarizationReason()
        {
            Inventarizations = new HashSet<Inventarization>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Inventarization> Inventarizations { get; set; }
    }
}
