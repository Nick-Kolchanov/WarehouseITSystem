using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class ProductToScreen
    {
        [DisplayName("null")]
        public int Id { get; set; }
        [DisplayName("null")]
        public int NomenclatureId { get; set; }
        [DisplayName("Наименование")]
        public string Name { get; set; } = "";
        [DisplayName("Тип")]
        public string Type { get; set; } = "";
        [DisplayName("null")]
        public Product.ProductStatus Status { get; set; }
        [DisplayName("Номер склада")]
        public int WarehouseId { get; set; } = 0;
        [DisplayName("Стоимость")]
        public decimal Worth { get; set; } = 0;
        [DisplayName("Количество")]
        public int Count { get; set; } = 0;
    }
}
