using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class InventarizationToScreen
    {
        [DisplayName("null")]
        public int Id { get; set; }
        [DisplayName("Дата начала")]
        public DateOnly StartDate { get; set; }
        [DisplayName("Дата окончания")]
        public DateOnly? EndDate { get; set; }
        [DisplayName("Id склада")]
        public int? WarehouseId { get; set; }
        [DisplayName("Причина")]
        public string? Reason { get; set; }
    }
}
