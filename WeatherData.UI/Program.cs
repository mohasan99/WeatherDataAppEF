using WeatherData.Core.Models;
using WeatherData.Core.Services;
using WeatherData.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WeatherData.UI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new WeatherContext(); // Creates a new instance of the database context

            // 1. Create Database if it doesn't exist
            db.Database.EnsureCreated();

            // 2. Import CSV only if DB is empty
            if (!db.WeatherEntries.Any())
            {
                Console.WriteLine("Importing CSV data...");
                var entries = CsvImporter.Import("TempFuktData.csv"); //Import data from CSV file
                db.WeatherEntries.AddRange(entries); //Add data to EF Core
                db.SaveChanges(); //Save changes to Database
                Console.WriteLine($"Imported {entries.Count} rows.");
            }

            // 1: Average temperature for a given date
            var allEntries = db.WeatherEntries.ToList();
            Console.Write("Enter a date (yyyy-mm-dd): ");
            var input = Console.ReadLine();

            if (!DateTime.TryParse(input, out var chosenDate))
            {
                Console.WriteLine("Invalid date format.");
                return;
            }

            var avgOutdoor = WeatherCalculator
            .GetAverageOutdoorTempForDate(allEntries, chosenDate);

            if (avgOutdoor.HasValue)
            {
                Console.WriteLine(
                    $"Average outdoor temperature on {chosenDate:yyyy-MM-dd} was {avgOutdoor:F1} °C");
            }
            else
            {
                Console.WriteLine("No outdoor data found for that date.");
            }

            // 2: Hottest to coldest outdoor days
            Console.WriteLine("\n--- Outdoor days sorted by average temperature (warmest to coldest) ---");

            // Call the calculator method
            var sortedDays = WeatherCalculator.GetOutdoorDailyAveragesSorted(allEntries);

            // Print the results
            foreach (var day in sortedDays)
            {
                Console.WriteLine($"{day.Date:yyyy-MM-dd}: {day.AverageTemperature:F1} °C");
            }

            // 3. Driest to most humid outdoor days
            Console.WriteLine($"Unique outdoor days: {sortedDays.Count}");

            Console.WriteLine("\n--- Outdoor days sorted by average humidity (driest to most humid) ---");

            var sortedHumidityDays = WeatherCalculator.GetOutdoorDailyHumiditySorted(allEntries);

            foreach (var day in sortedHumidityDays)
            {
                Console.WriteLine($"{day.Date:yyyy-MM-dd}: {day.AverageHumidity:F1} %");
            }

            // 4. Mold risk from low to high
            Console.WriteLine("\n--- Outdoor days sorted by mold risk (least to greatest) ---");

            // Table header
            Console.WriteLine($"{"Date",-12} {"Temp(°C)",-10} {"Hum(%)",-10} {"Risk",-10}");
            Console.WriteLine(new string('-', 45));

            var moldRiskDays = WeatherCalculator.GetOutdoorMoldRiskSorted(allEntries);

            // Table rows
            foreach (var day in moldRiskDays)
            {
                Console.WriteLine(
                    $"{day.Date:yyyy-MM-dd,-12} " +
                    $"{day.AverageTemperature,-10:F1}" +
                    $"{day.AverageHumidity,-10:F1}" +
                    $"{day.RiskIndex,-10:F2}"
                );
            }
            // 5. Meteorological autumn (outdoors)
            Console.WriteLine("\n--- Meteorological autumn (outdoors) ---");

            var autumnDate = WeatherCalculator.GetMeteorologicalAutumnDate(allEntries);

            if (autumnDate.HasValue)
            {
                Console.WriteLine($"Meteorological autumn starts on: {autumnDate.Value:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine("Meteorological autumn date could not be determined from this dataset.");
            }

            // 6. Meteorological winter (outdoors)
            Console.WriteLine("\n--- Meteorological winter (outdoors) ---");

            var winterDate = WeatherCalculator.GetMeteorologicalWinterDate(allEntries);

            if (winterDate.HasValue)
            {
                Console.WriteLine($"Meteorological winter starts on: {winterDate.Value:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine("Meteorological winter date could not be determined from this dataset.");
            }

            // 1. Indoor average temperature for a given date
            Console.WriteLine("\n--- Indoor average temperature for a given date ---");

            Console.Write("Enter a date (yyyy-MM-dd): ");
            var indoorInput = Console.ReadLine();

            // Validate input
            if (!DateTime.TryParse(indoorInput, out var indoorDate))
            {
                Console.WriteLine("Invalid date format.");
            }
            else
            {
                // Call the calculator
                var avgIndoorTemp = WeatherCalculator.GetAverageIndoorTempForDate(allEntries, indoorDate);

                if (avgIndoorTemp.HasValue)
                {
                    Console.WriteLine(
                        $"Average indoor temperature on {indoorDate:yyyy-MM-dd} was {avgIndoorTemp.Value:F1} °C");
                }
                else
                {
                    Console.WriteLine("No indoor data found for that date.");
                }
            }

            // 2. Indoor days sorted by average temperature 
            Console.WriteLine("\n--- Indoor days sorted by average temperature (warmest to coldest) ---\n");

            var sortedIndoorDays = WeatherCalculator.GetIndoorDailyAveragesSorted(allEntries);

            // Header
            Console.WriteLine($"{"Date",-12} {"AvgTemp(°C)",-12}");
            Console.WriteLine(new string('-', 26));

            // Rows
            foreach (var day in sortedIndoorDays)
            {
                Console.WriteLine($"{day.Date:yyyy-MM-dd,-12} {day.AverageTemperature,-12:F1}");
            }

            // 3. Indoor days sorted by average humidity
            Console.WriteLine("\n--- Indoor days sorted by average humidity (driest to most humid) ---\n");


            var sortedIndoorHumidity = WeatherCalculator.GetIndoorDailyHumiditySorted(allEntries);

            Console.WriteLine($"{"Date",-12} {"AvgHum(%)",-12}");
            Console.WriteLine(new string('-', 26));

            foreach (var day in sortedIndoorHumidity)
            {
                Console.WriteLine($"{day.Date:yyyy-MM-dd,-12} {day.AverageHumidity,-12:F1}");
            }

            // 4. Indoor days sorted by mold risk 
            Console.WriteLine("\n--- Indoor days sorted by mold risk (least to greatest) ---\n");


            var indoorMoldRiskDays = WeatherCalculator.GetIndoorMoldRiskSorted(allEntries);

            // Header
            Console.WriteLine($"{"Date",-12} {"Temp(°C)",-10} {"Hum(%)",-10} {"Risk",-10}");
            Console.WriteLine(new string('-', 45));

            // Rows
            foreach (var day in indoorMoldRiskDays)
            {
                Console.WriteLine(
                    $"{day.Date:yyyy-MM-dd,-12}" +
                    $"{day.AverageTemperature,-10:F1}" +
                    $"{day.AverageHumidity,-10:F1}" +
                    $"{day.RiskIndex,-10:F2}");
            }

        }

    }
}