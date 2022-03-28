using System.ComponentModel;

namespace WarehouseITSystem.Models
{
    public partial class PropertyToScreen
    {
        [DisplayName("Намиенование")]
        public string Name { get; set; } = "";
        [DisplayName("Значение")]
        public string Value { get; set; } = "";
        [DisplayName("Ед.изм.")]
        public string? MeasurementUnit { get; set; }
    }
}
