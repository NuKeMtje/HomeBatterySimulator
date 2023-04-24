# HomeBatterySimulator
Will try to predict the behaviour of a configurable battery and inverder system, based on supplied grid usage and solar production data.
The output contains a predection of the possible profit to be made based on the configured system.
One setting allows changing the gain of the solar panel input. This can be used to evaluate the effect of extra (or less) panels.

I'm not a programmer so feedback or corrections of my code are welcome.

## Input data and format
The needed input data are:
- Energy from the grid in kWh, only positive values. If energy is returned to the grid, the entry should be 0
- Energy to the grid in kWh, only positive values. If energy is consumed from the grid, the entry should be 0
- Energy supplied by the solar panels (or other) in kWh, only positive values

Each entry is based on a point in time. The frequency must be the same for all data (e.g. a value every 10,15 or 60 minutes).

Two csv-file input formats are supported. My code also contains a third, but this was for internal testing only and will/can be removed.
When running the program it will ask the user which format to use.

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

## Output
