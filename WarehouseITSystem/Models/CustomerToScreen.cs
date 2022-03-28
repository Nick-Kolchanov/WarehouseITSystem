using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class CustomerToScreen
    {
        [DisplayName("null")]
        public int Id { get; set; }
        [DisplayName("Имя")]
        public string? Name { get; set; }
        [DisplayName("Телефон")]
        public string Phone { get; set; } = null!;
        [DisplayName("Общая стоимость покупок")]
        public decimal SellingSum { get; set; } = 0;
    }
}
