using BatCalc.Domain.Models;
using BatCalc.Infrastructure.Csv;

namespace BatCalc.Domain
{
    public class BatSim
    {
        private readonly Settings _settings;

        public List<EnergyHistoryEntry> RelEnergyFromGrid { get; set; }
        public List<EnergyHistoryEntry> RelEnergyReturnedToGrid { get; set; }
        public List<EnergyHistoryEntry> RelEnergyFromGridSolarFactorCorrected { get; set; }
        public List<EnergyHistoryEntry> RelEnergyReturnedToGridSolarFactorCorrected { get; set; }
        public List<EnergyHistoryEntry> RelEnergyFromSolar { get; set; }
        public List<EnergyHistoryEntry> RelEnergyConsumption { get; set; }
        public List<ComparableEnergyHistoryEntry> BaseSet { get; set; }

        /// <summary>
        /// from grid, to grid, SOC
        /// </summary>
        public List<ComparableEnergyHistoryEntry> Result { get; set; }

        public BatSim(CsvDataExtractor extractInfo, Settings settings)
        {
            Result = new List<ComparableEnergyHistoryEntry>();
            _settings = settings;
            SetData(extractInfo);
        }
                
        public void SetData(CsvDataExtractor extractInfo)
        {
            RelEnergyFromGrid = extractInfo.RelEnergyFromGrid;
            RelEnergyReturnedToGrid = extractInfo.RelEnergyReturnedToGrid;
            RelEnergyFromSolar = extractInfo.RelEnergyFromSolar.Clone().ToList();
            RelEnergyConsumption = extractInfo.RelEnergyConsumption;

            //now set new situation
            RelEnergyFromSolar.ForEach(s => s.Energy = s.Energy * _settings.SolarFactor);
            CalculateBasedOnSolarFactor();

            BaseSet = Mapper.Join(RelEnergyConsumption, RelEnergyFromSolar).ToList();
        }

        public void Process()
        {
            var currentBatState = new BatState()
            {
                Timestamp = DateTimeOffset.MinValue,
                AvailableEnergy = 0,
            };

            var maxIn = _settings.MaxChargingPower; //kW to kWh one on one
            var maxOut = _settings.MaxDischargingPower; //kW to kWh one on one
            var usableCapacity = _settings.BatEnergyCapacity * _settings.BatUsable;

            foreach (var item in BaseSet.OrderBy( s=> s.Timestamp))
            {
                var fromGrid = 0m;
                var toGrid = 0m;

                var energy = item.Energy2 - item.Energy1;
                    
                //energy surplus
                if (energy > 0)
                {
                    //place in batt?
                    var freeInBat = (usableCapacity) - currentBatState.AvailableEnergy;
                    if (freeInBat > 0)
                    {
                        //Top battery and add excess to grid
                        if (energy > freeInBat && freeInBat <= maxIn)
                        {
                            toGrid += energy.Value - (freeInBat/ _settings.InputEfficienty);
                            currentBatState.Timestamp = item.Timestamp;
                            currentBatState.AvailableEnergy = usableCapacity;
                        }
                        //Top battery with max power and add excess to grid
                        else if (energy > freeInBat && freeInBat > maxIn || energy <= freeInBat && energy > maxIn)
                        {
                            toGrid += energy.Value - (maxIn/ _settings.InputEfficienty);
                            currentBatState.Timestamp = item.Timestamp;
                            currentBatState.AvailableEnergy = currentBatState.AvailableEnergy + maxIn;
                        }
                        //Top battery
                        else if (energy <= freeInBat && energy <= maxIn)
                        {
                            currentBatState.Timestamp = item.Timestamp;
                            currentBatState.AvailableEnergy = currentBatState.AvailableEnergy + (energy.Value* _settings.InputEfficienty);
                        }
                    }
                    else
                    {
                        toGrid += energy.Value;
                        currentBatState.Timestamp = item.Timestamp;
                        currentBatState.AvailableEnergy = usableCapacity;
                    }
                }
                //energy lack
                else
                {
                    energy = Math.Abs(energy.Value);
                    //something left in batt?
                    if (currentBatState.AvailableEnergy > 0)
                    {
                        var allDemandAvailableInBatt = currentBatState.AvailableEnergy - (energy.Value/ _settings.OutputEfficienty) >= 0;
                        //get it all from battery
                        if (allDemandAvailableInBatt && energy <= maxOut)
                        {
                            currentBatState.Timestamp = item.Timestamp;
                            currentBatState.AvailableEnergy = currentBatState.AvailableEnergy - (energy.Value / _settings.OutputEfficienty);
                        }
                        //discharge max from battery
                        if ((allDemandAvailableInBatt && energy > maxOut) || (!allDemandAvailableInBatt && energy.Value - currentBatState.AvailableEnergy > maxOut))
                        {
                            fromGrid += energy.Value - maxOut* _settings.OutputEfficienty;
                            currentBatState.Timestamp = item.Timestamp;
                            currentBatState.AvailableEnergy = currentBatState.AvailableEnergy - maxOut;
                        }
                        //empty bat
                        if (!allDemandAvailableInBatt && energy.Value - currentBatState.AvailableEnergy <= maxOut)
                        {
                            fromGrid += energy.Value - currentBatState.AvailableEnergy* _settings.OutputEfficienty;
                            currentBatState.Timestamp = item.Timestamp;
                            currentBatState.AvailableEnergy = 0;
                        }
                    }
                    else
                    {
                        fromGrid += energy.Value;
                        currentBatState.Timestamp = item.Timestamp;
                        currentBatState.AvailableEnergy = 0;
                    }
                        
                }
                    
                Result.Add(new ComparableEnergyHistoryEntry(item.Timestamp, fromGrid, toGrid, currentBatState.AvailableEnergy/usableCapacity, currentBatState.AvailableEnergy));

            }
        }

        private void CalculateBasedOnSolarFactor()
        {
            RelEnergyFromGridSolarFactorCorrected = new List<EnergyHistoryEntry>();
            RelEnergyReturnedToGridSolarFactorCorrected = new List<EnergyHistoryEntry>();

            var baseset = Mapper.Join(RelEnergyConsumption, RelEnergyFromGrid, RelEnergyReturnedToGrid, RelEnergyFromSolar).ToList();
            foreach (var item in baseset)
            {
                var energy = item.Energy1 - item.Energy4;
                //to grid
                if (energy < 0)
                {
                    RelEnergyFromGridSolarFactorCorrected.Add(new EnergyHistoryEntry(item.Timestamp, 0m));
                    RelEnergyReturnedToGridSolarFactorCorrected.Add(new EnergyHistoryEntry(item.Timestamp, Math.Abs(energy.Value)));
                }
                //from grid
                else
                {
                    RelEnergyFromGridSolarFactorCorrected.Add(new EnergyHistoryEntry(item.Timestamp, energy.Value));
                    RelEnergyReturnedToGridSolarFactorCorrected.Add(new EnergyHistoryEntry(item.Timestamp, 0m));
                }
            }
        }
    }
}
