using BatCalc.Domain.Models;
using BatCalc.Infrastructure.Csv;

namespace BatCalc.Domain
{
    public class Analyser
    {
        private readonly CsvDataExtractor _extractor;
        private readonly BatSim _batSim;

        public Analyser(CsvDataExtractor extractor, BatSim batSim)
        {
            _extractor = extractor;
            _batSim = batSim;
        }

        public Output BuildTotals(decimal elecUsePricekWh, decimal elecInjectPricekWh)
        {
            var output = new Output();

            output.EnergyConsumption = _batSim.RelEnergyConsumption.Sum(x => x.Energy);
            output.EnergyFromSolarOriginal = _extractor.RelEnergyFromSolar.Sum(x => x.Energy);
            output.EnergyFromSolarCorr = _batSim.RelEnergyFromSolar.Sum(x => x.Energy);
            output.EnergyToGridNoBat = _extractor.RelEnergyReturnedToGrid.Sum(x => x.Energy);
            output.EnergyFromGridNoBat = _extractor.RelEnergyFromGrid.Sum(x => x.Energy);
            output.EnergyToGridNoBatCorr = _batSim.RelEnergyReturnedToGridSolarFactorCorrected.Sum(x => x.Energy);
            output.EnergyFromGridNoBatCorr = _batSim.RelEnergyFromGridSolarFactorCorrected.Sum(x => x.Energy);
            output.EnergyToGridWithBat = _batSim.Result.Sum(x => x.Energy2.Value);
            output.EnergyFromGridWithBat = _batSim.Result.Sum(x => x.Energy1.Value);
            output.CostNoBat = output.EnergyFromGridNoBat * elecUsePricekWh - output.EnergyToGridNoBat * elecInjectPricekWh;
            output.CostNoBatCorr = output.EnergyFromGridNoBatCorr * elecUsePricekWh - output.EnergyToGridNoBatCorr * elecInjectPricekWh;
            output.CostWithBat = output.EnergyFromGridWithBat * elecUsePricekWh - output.EnergyToGridWithBat * elecInjectPricekWh;

            return output;
        }

        public Dictionary<int,Output> BuildMonthlyTotals(decimal elecUsePricekWh, decimal elecInjectPricekWh)
        {
            var result = new Dictionary<int,Output>();
            for (int month = 1; month <= 12; month++)
            {
                var output = new Output();

                output.EnergyConsumption = _batSim.RelEnergyConsumption.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy);
                output.EnergyFromSolarOriginal = _extractor.RelEnergyFromSolar.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy);
                output.EnergyFromSolarCorr = _batSim.RelEnergyFromSolar.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy);
                output.EnergyToGridNoBat = _extractor.RelEnergyReturnedToGrid.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy);
                output.EnergyFromGridNoBat = _extractor.RelEnergyFromGrid.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy);
                output.EnergyToGridNoBatCorr = _batSim.RelEnergyReturnedToGridSolarFactorCorrected.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy);
                output.EnergyFromGridNoBatCorr = _batSim.RelEnergyFromGridSolarFactorCorrected.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy);
                output.EnergyToGridWithBat = _batSim.Result.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy2.Value);
                output.EnergyFromGridWithBat = _batSim.Result.Where(e => e.Timestamp.Month == month).Sum(x => x.Energy1.Value);
                output.CostNoBat = output.EnergyFromGridNoBat * elecUsePricekWh - output.EnergyToGridNoBat * elecInjectPricekWh;
                output.CostNoBatCorr = output.EnergyFromGridNoBatCorr * elecUsePricekWh - output.EnergyToGridNoBatCorr * elecInjectPricekWh;
                output.CostWithBat = output.EnergyFromGridWithBat * elecUsePricekWh - output.EnergyToGridWithBat * elecInjectPricekWh;

                result.Add(month, output);
            }
            return result;
        }
    }
}
