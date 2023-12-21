namespace API.Models
{
    public record Weather(
        string ServiceName,
        string TemperatureC,
        string TemperatureF,
        string Cloud,
        string Humidity,
        string Precipitation,
        string SpeedOfWind,
        string DirectionOfWind
    )
    {
        public static string NoInfoIfNull(object? o)
        {
            return o?.ToString() ?? "No info";
        }

        public static double ConvertFromCToF(double c)
        {
            return 32 + c * 9 / 5;
        }
    }
}