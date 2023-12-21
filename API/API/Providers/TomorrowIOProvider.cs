using API.Models;

namespace API.Providers
{
    public class TomorrowIOProvider : IWeatherProvider
    {
        private string _name = "tomorrow_io";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Weather? GetCurrentWeather()
        {
            //var url = "https://api.tomorrow.io/v4/timelines?";
            var url = Environment.GetEnvironmentVariable("TOMORROW_IO_URL");

            var fields = new string[]
            {
                "temperature",
                "cloudCover",
                "humidity",
                "precipitationIntensity",
                "precipitationType",
                "windSpeed",
                "windDirection"
            };

            //var lat = 59.93863;
            //var lon = 30.31413;
            //var apikey = "944Ubw2jFtqMUZnVNXWInCkezgNumRK2";
            var lat = Environment.GetEnvironmentVariable("LAT");
            var lon = Environment.GetEnvironmentVariable("LON");
            var apikey = Environment.GetEnvironmentVariable("TOMORROW_IO_API_KEY");

            var parameters = new[] {
                $"location={lat},{lon}",
                $"fields={string.Join(",", fields)}",
                $"units=metric",
                $"apikey={apikey}"
            };

            var requestUrl = url + string.Join("&", parameters);
            try
            {
                var response = new HttpClient().GetFromJsonAsync<ResponseModels.TomorrowIO.Root>(requestUrl).Result;

                if (response == null)
                {
                    return null;
                }

                var current = response.data.timelines[0].intervals[0].values;

                var weather = new Weather(
                        ServiceName: _name,
                        TemperatureC: current.temperature.ToString(),
                        TemperatureF: Weather.ConvertFromCToF(current.temperature).ToString(),
                        Cloud: Weather.NoInfoIfNull(current?.cloudCover),
                        Humidity: Weather.NoInfoIfNull(current?.humidity),
                        DirectionOfWind: Weather.NoInfoIfNull(current?.windDirection),
                        SpeedOfWind: Weather.NoInfoIfNull(current?.windSpeed),
                        Precipitation: Weather.NoInfoIfNull(current?.precipitationIntensity)
                    );

                return weather;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}