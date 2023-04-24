using CsvHelper.Configuration.Attributes;

namespace BatCalc.Infrastructure.Csv
{
    public class CsvExportFormat
    {
        [Name("Timestamp")]
        public DateTime Timestamp { get; set; }
        [Name("FromGrid")]
        public decimal? FromGrid { get; set; }
        [Name("ToGrid")]
        public decimal? ToGrid { get; set; }
        [Name("Soc")]
        public decimal? Soc { get; set; }
        [Name("BatEnergy")]
        public decimal? BatEnergy { get; set; }
    }
}
