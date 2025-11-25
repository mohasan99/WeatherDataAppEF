using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData.Core.Models
{
    public class WeatherEntry
    {
        public int Id { get; set; } // Primary key for EF Core
        public string Place { get; set; } = ""; // "ute" / "inne"
        public DateTime Timestamp { get; set; }  // Date + Time
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
