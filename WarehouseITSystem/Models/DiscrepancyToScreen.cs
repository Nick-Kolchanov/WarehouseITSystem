using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class DiscrepancyToScreen
    {
        [DisplayName("null")]
        public int? InventarizationId { get; set; }
        [DisplayName("Id продукта")]
        public int? ProductId { get; set; }
        [DisplayName("Номенклатура")]
        public string? ProductName { get; set; }
        [DisplayName("Статус")]
        public string? Status { get; set; }
        [DisplayName("Тип")]
        public string? Type { get; set; }
    }
}
