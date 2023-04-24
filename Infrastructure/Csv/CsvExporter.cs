using BatCalc.Domain;
using BatCalc.Domain.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatCalc.Infrastructure.Csv
{
    public class CsvExporter
    {
        private readonly CsvDataExtractor _extractor;
        private readonly BatSim _batSim;
        private readonly Settings _settings;
        private readonly Output _output;
        private readonly Dictionary<int, Output> _outputMonthly;

        public CsvExporter(Settings settings, Output output, Dictionary<int, Output> outputMontly, CsvDataExtractor extractor, BatSim batSim)
        {
            _settings = settings;
            _output = output;
            _outputMonthly = outputMontly;
            _extractor = extractor;
            _batSim = batSim;
        }

        public void Store()
        {
            using (var writer = new StreamWriter(@$"Export_{_settings.Name}_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                OutputSettings(csv, _settings);
                OutputTotals(csv, _output);
                OutputMonths(csv, _outputMonthly, _output);
                var export = new List<CsvExportFormat>();
                foreach (var entry in _batSim.Result)
                {
                    export.Add(new CsvExportFormat()
                    {
                        Timestamp = entry.Timestamp.DateTime,
                        FromGrid = entry.Energy1,
                        ToGrid = entry.Energy2,
                        Soc = entry.Energy3,
                        BatEnergy = entry.Energy4,
                    });
                }
                csv.NextRecord();
                csv.WriteRecords(export);
            }
        }
        public void OutputSettings(CsvWriter writer, Settings settings)
        {
            settings.StringProperties().ToList().ForEach(i =>
            {
                writer.WriteComment(i);
                writer.NextRecord();
            });
        }

        public void OutputTotals(CsvWriter writer, Output output)
        {
            output.OutputTotals(() => writer.Configuration.Delimiter).ToList().ForEach(i =>
            {
                writer.WriteComment(i);
                writer.NextRecord();
            });
        }

        public void OutputMonths(CsvWriter writer, Dictionary<int, Output> output, Output total)
        {
            Output.OutputMonths(output, total, () => writer.Configuration.Delimiter).ToList().ForEach(i =>
            {
                writer.WriteComment(i);
                writer.NextRecord();
            });
        }
    }
}
