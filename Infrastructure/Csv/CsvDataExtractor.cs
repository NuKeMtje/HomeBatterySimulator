using BatCalc.Domain.Models;

namespace BatCalc.Infrastructure.Csv
{
    public class CsvDataExtractor
    {
        private List<EnergyHistoryEntry> EnergyFromGrid { get; set; }
        private List<EnergyHistoryEntry> EnergyReturnedToGrid { get; set; }
        private List<EnergyHistoryEntry> EnergyFromSolar { get; set; }

        public List<EnergyHistoryEntry> RelEnergyFromGrid { get; private set; }
        public List<EnergyHistoryEntry> RelEnergyReturnedToGrid { get; private set; }
        public List<EnergyHistoryEntry> RelEnergyFromSolar { get; private set; }
        public List<EnergyHistoryEntry> RelEnergyConsumption { get; private set; }

        public CsvDataExtractor()
        {

        }

        public void Process(List<CsvImportAllFormat> energyAll)
        {
            var fromGrid = new List<EnergyHistoryEntry>();
            var toGrid = new List<EnergyHistoryEntry>();
            var solar = new List<EnergyHistoryEntry>();
            energyAll.Map(ref fromGrid, ref toGrid, ref solar);
            EnergyFromGrid = fromGrid;
            EnergyReturnedToGrid = toGrid;
            EnergyFromSolar = solar;

            RelEnergyConsumption = CalcEnergyConsumption(EnergyFromGrid, EnergyReturnedToGrid, EnergyFromSolar);
        }

        public void Process(List<CsvImportOneFormat> energyFromGrid, List<CsvImportOneFormat> energyReturnedToGrid, List<CsvImportOneFormat> energyFromSolar)
        {
            EnergyFromGrid = energyFromGrid.Map();
            EnergyReturnedToGrid = energyReturnedToGrid.Map();
            EnergyFromSolar = energyFromSolar.Map();

            RelEnergyConsumption = CalcEnergyConsumption(EnergyFromGrid, EnergyReturnedToGrid, EnergyFromSolar);
        }

        public void Process(List<CsvImportTestFormat> energyFromGrid, List<CsvImportTestFormat> energyReturnedToGrid, List<CsvImportTestFormat> energyFromSolar)
        {
            EnergyFromGrid = energyFromGrid.Map();
            EnergyReturnedToGrid = energyReturnedToGrid.Map();
            EnergyFromSolar = energyFromSolar.Map();

            RelEnergyConsumption = CalcEnergyConsumption(EnergyFromGrid, EnergyReturnedToGrid, EnergyFromSolar);
        }

        private List<EnergyHistoryEntry> CalcEnergyConsumption(List<EnergyHistoryEntry> energyFromGrid, List<EnergyHistoryEntry> energyReturnedToGrid, List<EnergyHistoryEntry> energyFromSolar)
        {
            var result = new List<EnergyHistoryEntry>();

            RelEnergyFromGrid = energyFromGrid.ToRelative().ToList();
            RelEnergyReturnedToGrid = energyReturnedToGrid.ToRelative().ToList();
            RelEnergyFromSolar = energyFromSolar.ToRelative().ToList();

            var joined = Mapper.Join(RelEnergyFromGrid, RelEnergyReturnedToGrid, RelEnergyFromSolar).OrderBy(j => j.Timestamp).ToList();
            for (int i = 0; i < joined.Count; i++)
            {
                result.Add(new EnergyHistoryEntry(joined[i].Timestamp, Math.Abs(joined[i].Energy3.Value - joined[i].Energy2.Value + joined[i].Energy1.Value)));
            }
            return result;
        }
    }

    public static class Mapper
    {
        public static void Map(this List<CsvImportAllFormat> input, ref List<EnergyHistoryEntry> fromGrid, ref List<EnergyHistoryEntry> toGrid, ref List<EnergyHistoryEntry> solar)
        {
            foreach (var item in input)
            {
                if (item.FromGrid.HasValue && item.ToGrid.HasValue && item.Solar.HasValue)
                {
                    fromGrid.Add(new EnergyHistoryEntry(item.Timestamp, item.FromGrid.Value));
                    toGrid.Add(new EnergyHistoryEntry(item.Timestamp, item.ToGrid.Value));
                    solar.Add(new EnergyHistoryEntry(item.Timestamp, item.Solar.Value));
                }
            }
        }

        public static List<EnergyHistoryEntry> Map(this List<CsvImportOneFormat> input)
        {
            return input.Select(item => new EnergyHistoryEntry(item.Timestamp, item.Energy ?? 0)).ToList();
        }

        public static List<EnergyHistoryEntry> Map(this List<CsvImportTestFormat> input)
        {
            return input.Select(item => new EnergyHistoryEntry(item.Timestamp, item.State ?? 0)).ToList();
        }

        public static IEnumerable<ComparableEnergyHistoryEntry> Join(List<EnergyHistoryEntry> input1, List<EnergyHistoryEntry> input2)
        {
            return input1.Join(input2, i1 => i1.Timestamp, i2 => i2.Timestamp, (i1, i2) =>
            {
                return new ComparableEnergyHistoryEntry(i1.Timestamp, i1.Energy, i2.Energy);
            });
        }

        public static IEnumerable<ComparableEnergyHistoryEntry> Join(List<EnergyHistoryEntry> input1, List<EnergyHistoryEntry> input2, List<EnergyHistoryEntry> input3)
        {
            var result1 = input1.Join(input2, i1 => i1.Timestamp, i2 => i2.Timestamp, (i1, i2) =>
                {
                    return new ComparableEnergyHistoryEntry(i1.Timestamp, i1.Energy, i2.Energy);
                });
            var result2 = input3.Join(result1, i3 => i3.Timestamp, j1 => j1.Timestamp, (i3, j1) =>
            {
                return new ComparableEnergyHistoryEntry(j1.Timestamp, j1.Energy1, j1.Energy2, i3.Energy);
            });
            return result2;
        }

        public static IEnumerable<ComparableEnergyHistoryEntry> Join(List<EnergyHistoryEntry> input1, List<EnergyHistoryEntry> input2, List<EnergyHistoryEntry> input3, List<EnergyHistoryEntry> input4)
        {
            var result1 = input1.Join(input2, i1 => i1.Timestamp, i2 => i2.Timestamp, (i1, i2) =>
            {
                return new ComparableEnergyHistoryEntry(i1.Timestamp, i1.Energy, i2.Energy);
            });
            var result2 = input3.Join(result1, i3 => i3.Timestamp, j1 => j1.Timestamp, (i3, j1) =>
            {
                return new ComparableEnergyHistoryEntry(j1.Timestamp, j1.Energy1, j1.Energy2, i3.Energy);
            });
            var result3 = input4.Join(result2, i4 => i4.Timestamp, j2 => j2.Timestamp, (i4, j2) =>
            {
                return new ComparableEnergyHistoryEntry(j2.Timestamp, j2.Energy1, j2.Energy2, j2.Energy3, i4.Energy);
            });
            return result3;
        }

        public static IEnumerable<EnergyHistoryEntry> ToRelative(this List<EnergyHistoryEntry> input)
        {
            var result = new List<EnergyHistoryEntry>();
            for (int i = 1; i < input.Count; i++)
            {
                var previous = input[i - 1];
                var current = input[i];
                result.Add(new EnergyHistoryEntry(current.Timestamp, current.Energy - previous.Energy));
            }

            return result;
        }
    }
}
