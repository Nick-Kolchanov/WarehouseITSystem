using System;
using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class SellingToScreen
    {
        [DisplayName("Id")]
        public int Id { get; set; }
        [DisplayName("Дата")]
        public DateOnly Date { get; set; }
        [DisplayName("Скидка")]
        public int? PersonalDiscount { get; set; }

    }
}
