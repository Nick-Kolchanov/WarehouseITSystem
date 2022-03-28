using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class DateToScreen
    {
        [DisplayName("День")]
        public int Day { get; set; }
        [DisplayName("Месяц")]
        public int Month { get; set; } 
        [DisplayName("Год")]
        public int Year { get; set; }
    }
}
