namespace BatCalc.Domain.Models
{
    public class BatState
    {
        public DateTimeOffset Timestamp { get; set; }
        public decimal AvailableEnergy { get; set; }
    }
}
