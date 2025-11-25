## WeatherDataAppEF
EFWeatherDataApp is a C# console application that imports weather data from a CSV file, stores it using Entity Framework Core (Code First), and calculates various indoor and outdoor weather statistics such as temperature trends, humidity levels, mold risk, and meteorological seasons.

## Features
- Outdoor
  - Average temperature for any selected date
  - Warmest → coldest days (daily average temperature)
  - Driest → most humid days (daily average humidity)
  - Mold risk ranking
  - Meteorological Autumn date (first 5 days between 0–10°C)
  - Meteorological Winter date (first 5 days ≤ 0°C)*
  - Winter 2016 may not occur due to mild conditions

- Indoor
  - Average indoor temperature for selected date
  - Warmest → coldest indoor days
  - Driest → most humid indoor days
  - Indoor mold risk ranking
    
## Data Handling
- Imports data from TempFuktData.csv
- Automatically creates a SQLite database
- Only imports the CSV once (DB caching)
- Clean project structure using Core, DataAccess, and UI layers
  
## How to Run
- Clone or download the project
- Open the solution in Visual Studio
- Set WeatherData.UI as the startup project
- Place TempFuktData.csv in the UI project and set:
- Build Action: Content
  - Copy to Output Directory: Copy always
  - Run the app
  - The database is created automatically
  - CSV data imports on first run
  - Navigate statistics via simple console menu

## Technologies
- C# / .NET
- Entity Framework Core
- SQLite
- LINQ
