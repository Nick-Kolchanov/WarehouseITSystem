using System;
using System.Collections.Generic;

namespace WarehouseITSystem.Models
{
    public partial class Product
    {
        public enum ProductStatus
        { 
            На_приемке, 
            На_складе, 
            Продан, 
            Возвращен, 
            Списан,
        }  

        public int Id { get; set; }
        public int NomenclatureId { get; set; }
        public int? DeliveryId { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.На_приемке;
        public int? WarehouseId { get; set; }
        public string? CellAddress { get; set; }

        public virtual Delivery? Delivery { get; set; }
        public virtual Nomenclature? Nomenclature { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
    }
}
