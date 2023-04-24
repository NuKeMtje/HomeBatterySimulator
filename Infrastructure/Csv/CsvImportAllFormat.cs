using CsvHelper.Configuration.Attributes;


namespace BatCalc.Infrastructure.Csv
{
    public class CsvImportAllFormat
    {
        [Name("Timestamp")]
        public DateTime Timestamp { get; set; }
        [Name("FromGrid")]
        public decimal? FromGrid { get; set; }
        [Name("ToGrid")]
        public decimal? ToGrid { get; set; }
        [Name("Solar")]
        public decimal? Solar { get; set; }
    }
}
