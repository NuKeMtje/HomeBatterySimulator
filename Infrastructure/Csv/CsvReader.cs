using System.Globalization;
using System.Text;


namespace BatCalc.Infrastructure.Csv
{
    public class CsvReader
    {
        private List<Tuple<string, string>> _parts;
        public List<Tuple<string, List<CsvImportTestFormat>>> Parsed;

        public CsvReader()
        {
            
        }

        public void ProcessAll(CsvDataExtractor csvDataExtractor, string pathToAll)
        {
            var all = ReadAllFile(pathToAll);

            csvDataExtractor.Process(all);
        }
        public void Process(CsvDataExtractor csvDataExtractor, string pathToFromGrid, string pathToToGrid, string pathToSolar)
        {
            var fromGrid = ReadOneFile(pathToFromGrid);
            var toGrid = ReadOneFile(pathToToGrid);
            var solar = ReadOneFile(pathToSolar);

            csvDataExtractor.Process(fromGrid, toGrid, solar);
        }

        public void ProcessTest(CsvDataExtractor csvDataExtractor, string pathToAll)
        {
            Parsed = new List<Tuple<string, List<CsvImportTestFormat>>>();
            _parts = Split(pathToAll);
            foreach (Tuple<string, string> part in _parts)
            {
                using (var reader = new StringReader(part.Item2))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<CsvImportTestFormat>().ToList();
                    Parsed.Add(new Tuple<string, List<CsvImportTestFormat>>(part.Item1, records.OrderBy(r => r.Timestamp).ToList()));
                }
            }
            csvDataExtractor.Process(Parsed[1].Item2, Parsed[0].Item2, Parsed[2].Item2);
        }

        public List<Tuple<string, string>> Split(string path)
        {
            var result = new List<Tuple<string, string>>();

            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, path)))
            {
                var header = reader.ReadLine() + "\n"; ;
                string part = "";
                var title = "";
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;
                    if (line.Contains("sensor"))
                    {
                        if (!string.IsNullOrEmpty(part))
                            result.Add(new Tuple<string, string>(title, header + part));
                        part = "";
                        title = line;
                    }
                    else
                    {
                        part += line + "\n";
                    }
                }
                if (!string.IsNullOrEmpty(part))
                    result.Add(new Tuple<string, string>(title, header + part));
            }
            return result;
        }

        public List<CsvImportOneFormat> ReadOneFile(string path)
        {
            var builder = new StringBuilder();
            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, path)))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CsvImportOneFormat>().ToList();
                return records.OrderBy(r => r.Timestamp).ToList();
            }
        }

        public List<CsvImportAllFormat> ReadAllFile(string path)
        {
            var builder = new StringBuilder();
            using (var reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, path)))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CsvImportAllFormat>().ToList();
                return records.OrderBy(r => r.Timestamp).ToList();
            }
        }
    }
}
