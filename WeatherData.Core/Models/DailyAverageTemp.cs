using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherData.Core.Models
{
    public class DailyAverageTemp 
    {
        public DateTime Date { get; set; }
        public double AverageTemperature { get; set; }
    }
}
