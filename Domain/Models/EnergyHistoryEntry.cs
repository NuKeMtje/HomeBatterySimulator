namespace BatCalc.Domain.Models
{
    public class EnergyHistoryEntry : ICloneable
    {
        public DateTimeOffset Timestamp { get; set; }
        public decimal Energy { get; set; }

        public EnergyHistoryEntry(DateTimeOffset timestamp, decimal energy)
        {
            Timestamp = timestamp;
            Energy = energy;
        }

        public object Clone()
        {
            return new EnergyHistoryEntry(Timestamp, Energy);
        }
    }

    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
