using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherData.Core.Models;


namespace WeatherData.DataAccess
{
    public class WeatherContext : DbContext // Database context - inherits from DbContext
    {
        public DbSet<WeatherEntry> WeatherEntries { get; set; } // Represents a table in the database

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=weather.db");
        }
    }
}
