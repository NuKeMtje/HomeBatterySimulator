
using BatCalc.Domain;
using BatCalc.Infrastructure;
using BatCalc.Infrastructure.Csv;

var configLoader = new SettingsReader();
var settingsList = configLoader.LoadSettings();

var extractor = new CsvDataExtractor();
var reader = new CsvReader();
var presenter = new Presentation();
var writeToFile = presenter.AskWriteToFile();
var inputMethod = presenter.AskInputMethod();

if (inputMethod == Enums.InputMethod.SeparateFiles)
{
    reader.Process(extractor, @"FromGrid.csv", @"ToGrid.csv", @"Solar.csv");
}
else if (inputMethod == Enums.InputMethod.OneFileWithAll)
{
    reader.ProcessAll(extractor, @"AllData.csv");
}
else
    reader.ProcessTest(extractor, @"entities-2023-04-21_08 39 48.csv");

foreach (var settings in settingsList)
{
    var batSim = new BatSim(extractor, settings);
    batSim.Process();
    var analyser = new Analyser(extractor, batSim);
    var output = analyser.BuildTotals(settings.ElecUsePricekWh, settings.ElecInjectPricekWh);
    var outputMonthly = analyser.BuildMonthlyTotals(settings.ElecUsePricekWh, settings.ElecInjectPricekWh);
    
    if (writeToFile)
    {
        var csvExporter = new CsvExporter(settings, output, outputMonthly, extractor, batSim);
        csvExporter.Store();
    }

    presenter.OutputAll(settings, output, outputMonthly);
}
Console.ReadLine();

