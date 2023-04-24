using System.Text;

namespace BatCalc.Domain.Models
{
    public class Settings
    {
        public string Name { get; set; }
        public decimal BatEnergyCapacity { get; set; }
        public decimal MaxChargingPower { get; set; }
        public decimal MaxDischargingPower { get; set; }
        public decimal BatUsable { get; set; }
        public decimal InputEfficienty { get; set; }
        public decimal OutputEfficienty { get; set; }
        public decimal SolarFactor { get; set; }
        
        public decimal ElecUsePricekWh { get; set; }
        public decimal ElecInjectPricekWh { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var item in StringProperties()) 
            {
                builder.Append(item.ToString());
                builder.AppendLine();
            }
            return builder.ToString();
        }

        public IEnumerable<string> StringProperties()
        {
            yield return $"---===  Settings  ===---";
            yield return $"Battery energy capacity (kWh): {BatEnergyCapacity}";
            yield return $"Max charging power (kW): {MaxChargingPower}";
            yield return $"Max discharging power (kW): {MaxDischargingPower}";
            yield return $"Bat SoC-range use (%): {BatUsable}";
            yield return $"Bat inverter input efficienty (%): {InputEfficienty}";
            yield return $"Bat inverter output efficienty (%)): {OutputEfficienty}";
            yield return $"Gain on solar production (factor): {SolarFactor}";
            yield return $"Elec price (euro/kWh): {ElecUsePricekWh}";
            yield return $"Elec injection price (euro/kWh): {ElecInjectPricekWh}";
            yield return "====================================================================================";
        }
    }
}
