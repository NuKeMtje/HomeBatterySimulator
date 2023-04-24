namespace BatCalc.Domain.Models
{
    public class Output
    {
        public decimal EnergyConsumption { get; set; }
        public decimal EnergyFromSolarOriginal { get; set; }
        public decimal EnergyFromSolarCorr { get; set; }
        public decimal EnergyToGridNoBat { get; set; }
        public decimal EnergyFromGridNoBat { get; set; }
        public decimal EnergyToGridNoBatCorr { get; set; }
        public decimal EnergyFromGridNoBatCorr { get; set; }
        public decimal EnergyToGridWithBat { get; set; }
        public decimal EnergyFromGridWithBat { get; set; }
        public decimal CostNoBat { get; set; }
        public decimal CostNoBatCorr { get; set; }
        public decimal CostWithBat { get; set; }


        public IEnumerable<string> OutputTotals(Func<string> delimiter)
        {
            yield return $"\n---===  General detailed overview  ===---\n";
            yield return $"Total energy consumption: {delimiter.Invoke()}{EnergyConsumption:#####} kWh";
            yield return $"Total energy from solar: {delimiter.Invoke()}{EnergyFromSolarOriginal:#####} kWh";
            yield return $"Total energy from solar corrected: {delimiter.Invoke()}{EnergyFromSolarCorr:#####} kWh";
            yield return $"---Before bat (original)---";
            yield return $"\tTotal energy to grid: {delimiter.Invoke()}{EnergyToGridNoBat:#####} kWh";
            yield return $"\tTotal energy from grid: {delimiter.Invoke()} {EnergyFromGridNoBat:#####} kWh";
            yield return $"---Before bat (solar factor corrected)---";
            yield return $"\tTotal energy to grid: {delimiter.Invoke()} {EnergyToGridNoBatCorr:#####} kWh";
            yield return $"\tTotal energy from grid: {delimiter.Invoke()} {EnergyFromGridNoBatCorr:#####} kWh";
            yield return $"---After bat---";
            yield return $"\tTotal energy to grid: {delimiter.Invoke()} {EnergyToGridWithBat:#####} kWh";
            yield return $"\tTotal energy from grid: {delimiter.Invoke()} {EnergyFromGridWithBat:#####} kWh";
            yield return $"---Costs---";
            yield return $"\tNo bat: {delimiter.Invoke()}{CostNoBat:####} euro";
            yield return $"\tNo bat corr: {delimiter.Invoke()}{CostNoBatCorr:####} euro";
            yield return $"\tBat: {delimiter.Invoke()}{CostWithBat:####} euro";
            yield return $"---Gain---";
            yield return $"\tGain: {delimiter.Invoke()}{CostNoBat - CostWithBat:####} euro";
            yield return $"\tGain corr: {delimiter.Invoke()}{CostNoBatCorr - CostWithBat:####} euro";
            yield return "====================================================================================";
        }

        public static IEnumerable<string> OutputMonths(Dictionary<int, Output> output, Output total, Func<string> delimiter)
        {
            yield return $"\n---===  On monthly base  ===---\n";
            yield return $"Month{delimiter.Invoke()}NoBat (corr){delimiter.Invoke()}{delimiter.Invoke()}Bat{delimiter.Invoke()}{delimiter.Invoke()}{delimiter.Invoke()}Gains";
            yield return $"{delimiter.Invoke()}FromGrid{delimiter.Invoke()}ToGrid{delimiter.Invoke()}FromGrid{delimiter.Invoke()}ToGrid{delimiter.Invoke()}{delimiter.Invoke()}Gain{delimiter.Invoke()}GainCorr{delimiter.Invoke()}";
            yield return $"{delimiter.Invoke()}kWh{delimiter.Invoke()}{delimiter.Invoke()}kWh{delimiter.Invoke()}kWh{delimiter.Invoke()}{delimiter.Invoke()}kWh{delimiter.Invoke()}{delimiter.Invoke()}euro{delimiter.Invoke()}euro{delimiter.Invoke()}";
            for (int month = 1; month <= 12; month++)
            {
                if (output.TryGetValue(month, out var monthOutput))
                    yield return OutputTabbed(month.ToString(), monthOutput, delimiter);
            }
            yield return "Total_______________________________________________________________________________";
            yield return OutputTabbed("", total, delimiter);
            yield return "====================================================================================";
        }

        private static string OutputTabbed(string month, Output output, Func<string> delimiter)
        {
            return $"{month} {delimiter.Invoke()}{output.EnergyFromGridNoBatCorr:###}{delimiter.Invoke()}{delimiter.Invoke()}{output.EnergyToGridNoBatCorr:###}{delimiter.Invoke()}{output.EnergyFromGridWithBat:###}{delimiter.Invoke()}{delimiter.Invoke()}{output.EnergyToGridWithBat:###}{delimiter.Invoke()}{delimiter.Invoke()}{output.CostNoBat - output.CostWithBat:###} {delimiter.Invoke()} {output.CostNoBatCorr - output.CostWithBat:###}";
        }
    }
}
