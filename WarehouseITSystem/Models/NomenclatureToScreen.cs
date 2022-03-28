﻿using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class NomenclatureToScreen
    {
        [DisplayName("null")]
        public int Id { get; set; }
        [DisplayName("Наименование")]
        public string Name { get; set; } = "";
        [DisplayName("Тип")]
        public string? Type { get; set; }
        [DisplayName("Стоимость")]
        public decimal Worth { get; set; } 
    }
}
