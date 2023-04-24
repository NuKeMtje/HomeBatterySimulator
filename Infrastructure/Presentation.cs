using BatCalc.Domain.Models;


namespace BatCalc.Infrastructure
{
    public class Presentation
    {
        public void OutputAll(Settings settings, Output output, Dictionary<int, Output> outputMontly)
        {
            OutputSettings(settings);
            OutputTotals(output);
            OutputMonths(outputMontly,output);
        }

        public void OutputSettings(Settings settings)
        {
            Console.WriteLine($"\n");
            settings.StringProperties().ToList().ForEach(i => Console.WriteLine(i));
        }
        public void OutputTotals(Output output)
        {
            Console.WriteLine($"\n");
            output.OutputTotals(() => " ").ToList().ForEach(i => Console.WriteLine(i));
        }

        public void OutputMonths(Dictionary<int, Output> output, Output total)
        {
            Console.WriteLine($"\n");
            Output.OutputMonths(output, total, () => "\t").ToList().ForEach(i => Console.WriteLine(i));
        }

        public bool AskWriteToFile()
        {
            Console.WriteLine("Write batsim conclusions to CSV? (default Y) (Y/N): ");
            return Console.ReadLine()?.ToUpper() != "N";
        }
        public Enums.InputMethod AskInputMethod()
        {
            Console.WriteLine("One file with FromGrid, ToGrid and Solar? (default Y) (Y/N): ");
            if (Console.ReadLine()?.ToUpper() != "N")
                return Enums.InputMethod.OneFileWithAll;
            Console.WriteLine("Seperate data files for FromGrid, ToGrid and Solar? (default Y) (Y/N): ");
            if (Console.ReadLine()?.ToUpper() != "N")
                return Enums.InputMethod.SeparateFiles;
            return Enums.InputMethod.Test;
        }
    }
}
