# HomeBatterySimulator
Will try to predict the behavior of a configurable battery and inverter system, based on supplied grid usage and solar production data.
The output contains a prediction of the possible profit to be made based on the configured system.
One setting allows changing the gain of the solar panel input. This can be used to evaluate the effect of extra (or less) panels.

I'm not a programmer so feedback on or corrections of my code are welcome.

## Input data and format
The needed input data are:
- Energy from the grid in kWh, only positive values. If energy is returned to the grid, the entry should be 0
- Energy to the grid in kWh, only positive values. If energy is consumed from the grid, the entry should be 0
- Energy supplied by the solar panels (or other) in kWh, only positive values

Each entry is based on a point in time. The frequency must be the same for all data (e.g. a value every 10,15 or 60 minutes).

Two csv-file input formats are supported. My code also contains a third, but this was for internal testing only and will/can be removed.
When running the program, it will ask the user which format to use.

I supplied some example input files for testing.
### One file
Named AllData.csv in the executable directory.
Format standard example:
```
Timestamp,FromGrid,ToGrid,Solar
2022-09-16 08:00:00,0.16,0,0.04
2022-09-16 09:00:00,0.51,0,0.31
2022-09-16 10:00:00,0.52,0.18,0.95
```
### For each input a file
Named FromGrid.csv, ToGrid.csv and Solar.csv in the executable directory.
Format standard example:
```
Timestamp,Energy
2022-09-16 08:00:00,0.16
2022-09-16 09:00:00,0.51
2022-09-16 10:00:00,0.52
```
## Settings
Setting up the configuration is done via variables in a JSON-file. Multiple setups can be supplied, a simulation will be performed for each one of them.
Please see the supplied example for details, but the key setup values are:
```
    "Name" : "Victron 5000 and BYD LVL",   #used in the output en export
    "BatEnergyCapacity": 15.4,             #in kWh
    "MaxChargingPower": 3.3,               #in kW
    "MaxDischargingPower": 3.3,            #in kW
    "BatUsable": 0.85,                     #factor, defines what range of the usable battery energy you want to use. To lengthen battery life, this can be lowered.
    "InputEfficienty": 0.92,               #factor, defines the energy input efficiency (there are losses in the inverter, battery, ...)
    "OutputEfficienty": 0.92,              #factor, defines the energy output efficiency (there are losses in the inverter, battery, ...)
    "SolarFactor": 2,                      #factor used to see the effect of a bigger or smaller solar input (default should be 1)
    "ElecUsePricekWh": 0.28,               #currency/kWh for using energy from the grid
    "ElecInjectPricekWh": 0.08             #currency/kWh for giving energy back to the grid (used in some countries).
```

## Output on the console
Example output on the console. This will be done for each setting defined in the settings.json.
When you see 'corrected' or 'corr', this means the value incorporates the solar gain setting.
```
---===  Settings  ===---
Battery energy capacity (kWh): 15,4
Max charging power (kW): 3,3
Max discharging power (kW): 3,3
Bat SoC-range use (%): 0,85
Bat inverter input efficienty (%): 0,92
Bat inverter output efficienty (%)): 0,92
Gain on solar production (factor): 2
Elec price (euro/kWh): 0,28
Elec injection price (euro/kWh): 0,08
====================================================================================



---===  General detailed overview  ===---

Total energy consumption:  3343 kWh
Total energy from solar:  1427 kWh
Total energy from solar corrected:  2853 kWh
---Before bat (original)---
        Total energy to grid:  605 kWh
        Total energy from grid:   2520 kWh
---Before bat (solar factor corrected)---
        Total energy to grid:   1597 kWh
        Total energy from grid:   2108 kWh
---After bat---
        Total energy to grid:   530 kWh
        Total energy from grid:   1193 kWh
---Costs---
        No bat:  657 euro
        No bat corr:  463 euro
        Bat:  292 euro
---Gain---
        Gain:  365 euro
        Gain corr:  171 euro
====================================================================================



---===  On monthly base  ===---

Month   NoBat (corr)            Bat                     Gains
        FromGrid        ToGrid  FromGrid        ToGrid          Gain    GainCorr
        kWh             kWh     kWh             kWh             euro    euro
1       377             76      314             1               29       12
2       285             215     164             63              48       22
3       296             262     130             75              67       32
4       121             298     16              168             50       19
5
6
7
8
9       110             189     15              65              37       17
10      246             358     35              141             72       42
11      304             137     202             17              41       19
12      369             61      318                             22       10
Total_______________________________________________________________________________
        2108            1597    1193            530             365      171
====================================================================================
```

## Output in CSV-file
During start-up you will be askes if you want to create CSV-files containing the results.
Each setting will result in a new CSV-file with a filename containing the defined setting name and looking like this: Export_Victron 5000 and BYD_2023-26-4--13-30-15
The file contains the same information as that on the console, but also includes the full data result of the simulator, containing:
- Timestamp
- FromGrid [kWh]
- ToGrid [kWh]
- Battery Soc [%], this is the calculated 'Energy in the battery' / (settings.BatEnergyCapacity * settings.BatUsable)
- Energy in the battery [kWh]

For example:
```
Timestamp,FromGrid,ToGrid,Soc,BatEnergy
09/16/2022 08:00:00,0.12,0,0,0
09/16/2022 09:00:00,0.08,0,0,0
09/16/2022 10:00:00,0,0,0.0569289533995416348357524828,0.7452
09/16/2022 11:00:00,0,0,0.1581359816653934300993124523,2.0700
09/16/2022 12:00:00,0,0,0.2909702062643239113827349121,3.8088
09/16/2022 13:00:00,0,0,0.4315355233002291825821237586,5.6488
09/16/2022 14:00:00,0,0,0.602322383498854087089381207,7.8844
```

