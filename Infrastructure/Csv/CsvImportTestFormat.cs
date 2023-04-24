using CsvHelper.Configuration.Attributes;


namespace BatCalc.Infrastructure.Csv
{
    public class CsvImportTestFormat
    {
        [Name("Time stamp")]
        public DateTime Timestamp { get; set; }
        [Name("State")]
        public decimal? State { get; set; }
        [Name("Mean")]
        public decimal? Mean { get; set; }
        [Name("Min")]
        public decimal? Min { get; set; }
        [Name("Max")]
        public decimal? Max { get; set; }
    }
}
