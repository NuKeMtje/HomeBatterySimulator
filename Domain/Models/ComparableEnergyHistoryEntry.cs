namespace BatCalc.Domain.Models
{
    public class ComparableEnergyHistoryEntry
    {
        public DateTimeOffset Timestamp { get; set; }
        public decimal? Energy1 { get; set; }
        public decimal? Energy2 { get; set; }
        public decimal? Energy3 { get; set; }
        public decimal? Energy4 { get; set; }

        public ComparableEnergyHistoryEntry(DateTimeOffset timestamp, decimal? energy1, decimal? energy2, decimal? energy3 = null, decimal? energy4 = null)
        {
            Timestamp = timestamp;
            Energy1 = energy1;
            Energy2 = energy2;
            Energy3 = energy3;
            Energy4 = energy4;
        }
    }
}
