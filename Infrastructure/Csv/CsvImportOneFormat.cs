using CsvHelper.Configuration.Attributes;


namespace BatCalc.Infrastructure.Csv
{
    public class CsvImportOneFormat
    {
        [Name("Timestamp")]
        public DateTime Timestamp { get; set; }
        [Name("Energy")]
        public decimal? Energy { get; set; }
    }
}
