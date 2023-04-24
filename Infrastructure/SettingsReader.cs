using BatCalc.Domain.Models;
using System.Text.Json;


namespace BatCalc.Infrastructure
{
    public class SettingsReader
    {
        public List<Settings> LoadSettings()
        {
            string fileName = "Settings.json";
            string jsonString = File.ReadAllText(fileName);
            var settings = JsonSerializer.Deserialize<List<Settings>>(jsonString);
            if (settings == null)
                throw new Exception($"{fileName} could not be read");
            return settings;
        }
    }
}
