using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData.Core.Models;


namespace WeatherData.DataAccess
{
    public static class CsvImporter
    {
        public static List<WeatherEntry> Import(string path)
        {
            var list = new List<WeatherEntry>();
            var inv = CultureInfo.InvariantCulture;

            foreach (var line in File.ReadLines(path).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length < 4) // Safety check
                    continue;

                // Safer parsing with TryParse:
                if (!DateTime.TryParse(parts[0], inv, DateTimeStyles.None, out var timestamp))
                    continue;

                if (!double.TryParse(parts[2], NumberStyles.Float, inv, out var temperature))
                    continue;

                if (!double.TryParse(parts[3], NumberStyles.Float, inv, out var humidity))
                    continue;

                var entry = new WeatherEntry
                {
                    Timestamp = timestamp,
                    Place = parts[1],
                    Temperature = temperature,
                    Humidity = humidity
                };

                list.Add(entry);
            }

            return list;
        }
    }
}


