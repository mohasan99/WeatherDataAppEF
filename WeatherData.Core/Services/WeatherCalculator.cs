using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData.Core.Models;

namespace WeatherData.Core.Services
{
    public static class WeatherCalculator
    {
        public static WeatherEntry GetHottestDay(IEnumerable<WeatherEntry> data) // Flexible method 
        {
            return data.OrderByDescending(x => x.Temperature).First(); // Sorts rows by temperature
        }

        // Calculate average outdoor temperature for a specific date
        public static double? GetAverageOutdoorTempForDate(IEnumerable<WeatherEntry> data, DateTime date)
        {
            var outdoorForDate = data
              .Where(entry => entry.Place == "Ute" && entry.Timestamp.Date == date.Date); //Filter for only "ute" and specific date

            if (!outdoorForDate.Any())
            {
                return null;
            }

            return outdoorForDate.Average(entry => entry.Temperature);

        }

        // Calculate daily average outdoor temperatures for all dates in the dataset
        public static List<DailyAverageTemp> GetOutdoorDailyAveragesSorted(IEnumerable<WeatherEntry> data)
        {
            var outdoorEntries = data
            .Where(entry => entry.Place == "Ute"); // Filter for only outdoor entries

            var groupedByDate = outdoorEntries
            .GroupBy(entry => entry.Timestamp.Date); // Group entries by date

            var dailyAverages = groupedByDate
             .Select(group => new DailyAverageTemp // Calculate average temperature for each date
             {
                 Date = group.Key,
                 AverageTemperature = group.Average(entry => entry.Temperature)
             });

            // Sort the daily averages in descending order
            var sorted = dailyAverages
            .OrderByDescending(day => day.AverageTemperature)
               .ToList();

            return sorted;
        }
        // Calculate daily average outdoor humidity for all dates in the dataset
        public static List<DailyAverageHumidity> GetOutdoorDailyHumiditySorted(IEnumerable<WeatherEntry> data)
        {
            var outdoorEntries = data
            .Where(entry => entry.Place == "Ute");

            var groupedByDate = outdoorEntries
            .GroupBy(entry => entry.Timestamp.Date);

            var dailyHumidity = groupedByDate
            .Select(group => new DailyAverageHumidity // Calculate average humidity for each date
            {
                Date = group.Key,
                AverageHumidity = group.Average(entry => entry.Humidity)
            });

            var sorted = dailyHumidity
               .OrderBy(day => day.AverageHumidity)   // ascending = driest first
                .ToList();

            return sorted;
        }
        // Calculate daily mold risk for all dates in the dataset
        public static List<DailyMoldRisk> GetOutdoorMoldRiskSorted(IEnumerable<WeatherEntry> data)
        {
            var outdoorEntries = data
            .Where(entry => entry.Place == "Ute");

            var groupedByDate = outdoorEntries
            .GroupBy(entry => entry.Timestamp.Date);

            var dailyRisks = groupedByDate
            .Select(group =>
            {
                var avgTemp = group.Average(entry => entry.Temperature); // Calculate average temperature for each date
                var avgHum = group.Average(entry => entry.Humidity); // Calculate average humidity for each date

                double riskIndex;

                // Simple mold risk model:
                // Too dry (<75%) or too cold (<0°C) => no risk
                if (avgHum < 75 || avgTemp < 0)
                {
                    riskIndex = 0;
                }

                else
                {
                    // More humidity above 70% and higher temp increases risk
                    riskIndex = (avgHum - 70) * (avgTemp / 10.0);
                }

                return new DailyMoldRisk
                {
                    Date = group.Key,
                    AverageTemperature = avgTemp,
                    AverageHumidity = avgHum,
                    RiskIndex = riskIndex
                };
            });
            // Sort by risk index ascending (lowest risk first)
            var sorted = dailyRisks
            .OrderBy(day => day.RiskIndex)
            .ToList();

            return sorted;

        }
        // Determine the start date of meteorological autumn
        public static DateTime? GetMeteorologicalAutumnDate(IEnumerable<WeatherEntry> data)
        {
            var outdoorEntries = data
           .Where(entry => entry.Place == "Ute");

            // Group by date and calculate daily average temperature
            var dailyTemps = outdoorEntries
            .GroupBy(entry => entry.Timestamp.Date)
            .Select(group => new DailyAverageTemp
            {
                Date = group.Key,
                AverageTemperature = group.Average(entry => entry.Temperature)
            })
            .OrderBy(day => day.Date)
            .ToList();

            // Look for first occurrence of 5 consecutive days with average temp between 0 and 10 °C
            if (dailyTemps.Count < 5)
                return null;

            for (int i = 0; i <= dailyTemps.Count - 5; i++)
            {
                // Take 5 consecutive days starting at i
                var window = dailyTemps.Skip(i).Take(5);

                // Check: all 5 days between 0 and 10 °C (0 < temp <= 10)
                bool allInRange = window.All(day =>
                    day.AverageTemperature > 0 && day.AverageTemperature <= 10);

                if (allInRange)
                {
                    // First day of the first valid 5-day period = autumn start
                    return dailyTemps[i].Date;
                }
            }

            return null;
        }
        public static DateTime? GetMeteorologicalWinterDate(IEnumerable<WeatherEntry> data)
        {

            var outdoorEntries = data
                .Where(entry => entry.Place == "Ute");

            // Build daily average temperatures
            var dailyTemps = outdoorEntries
                .GroupBy(entry => entry.Timestamp.Date)
                .Select(dayGroup => new DailyAverageTemp
                {
                    Date = dayGroup.Key,
                    AverageTemperature = dayGroup.Average(entry => entry.Temperature)
                })
                .OrderBy(day => day.Date)
                .ToList();

            // Need at least 5 days for a valid winter period
            if (dailyTemps.Count < 5)
                return null;

            //  Slide a 5-day window
            for (int i = 0; i <= dailyTemps.Count - 5; i++)
            {
                var window = dailyTemps.Skip(i).Take(5);

                // Winter rule: all 5 average temps <= 0°C
                bool allWinter = window.All(day => day.AverageTemperature <= 0);

                if (allWinter)
                {
                    // First day in window = winter start
                    return dailyTemps[i].Date;
                }
            }

            // No winter period found
            return null;
        }
        // Calculate average indoor temperature for a specific date
        public static double? GetAverageIndoorTempForDate(IEnumerable<WeatherEntry> data, DateTime date)
        {
            // 1. Filter to indoor entries on the chosen date
            var indoorTempsForDate = data
                .Where(entry => entry.Place == "Inne" && entry.Timestamp.Date == date.Date)
                .Select(entry => entry.Temperature)
                .ToList();

            // 2. If no indoor data for that date -> return null
            if (!indoorTempsForDate.Any())
            {
                return null;
            }

            // 3. Otherwise return the average temperature
            return indoorTempsForDate.Average();
        }
        public static List<DailyAverageTemp> GetIndoorDailyAveragesSorted(IEnumerable<WeatherEntry> data)
        {
            //  Only indoor measurements
            var indoorEntries = data
                .Where(entry => entry.Place == "Inne");

            // Group by date and compute daily averages
            var groupedByDate = indoorEntries
                .GroupBy(entry => entry.Timestamp.Date);

            var dailyAverages = groupedByDate
                .Select(dayGroup => new DailyAverageTemp
                {
                    Date = dayGroup.Key,
                    AverageTemperature = dayGroup.Average(entry => entry.Temperature)
                });

            //  Sort from warmest to coldest
            var sorted = dailyAverages
                .OrderByDescending(day => day.AverageTemperature)
                .ToList();

            return sorted;
        }
        // Calculate daily average indoor humidity for all dates in the dataset
        public static List<DailyAverageHumidity> GetIndoorDailyHumiditySorted(IEnumerable<WeatherEntry> data)
        {

            var indoorEntries = data
                .Where(entry => entry.Place == "Inne");


            var groupedByDate = indoorEntries
                .GroupBy(entry => entry.Timestamp.Date);

            // Compute daily average humidity
            var dailyHumidity = groupedByDate
                .Select(dayGroup => new DailyAverageHumidity
                {
                    Date = dayGroup.Key,
                    AverageHumidity = dayGroup.Average(entry => entry.Humidity)
                });


            var sorted = dailyHumidity
                .OrderBy(day => day.AverageHumidity)
                .ToList();

            return sorted;

        }

        public static List<DailyMoldRisk> GetIndoorMoldRiskSorted(IEnumerable<WeatherEntry> data)
        {

            var indoorEntries = data
                .Where(entry => entry.Place == "Inne");


            var dailyRisks = indoorEntries
                .GroupBy(entry => entry.Timestamp.Date)
                .Select(dayGroup =>
                {
                    var avgTemp = dayGroup.Average(entry => entry.Temperature);
                    var avgHum = dayGroup.Average(entry => entry.Humidity);

                    double riskIndex;

                    // Same simple mold risk model as outdoors:
                    // Too dry (<75%) or too cold (<0°C) => no risk
                    if (avgHum < 75 || avgTemp < 0)
                    {
                        riskIndex = 0;
                    }
                    else
                    {
                        // More humidity above 70% and higher temp increases risk
                        riskIndex = (avgHum - 70) * (avgTemp / 10.0);
                    }

                    return new DailyMoldRisk
                    {
                        Date = dayGroup.Key,
                        AverageTemperature = avgTemp,
                        AverageHumidity = avgHum,
                        RiskIndex = riskIndex
                    };
                });

            // Sort from least to greatest risk
            return dailyRisks
                .OrderBy(day => day.RiskIndex)
                .ToList();
        }

    }
}