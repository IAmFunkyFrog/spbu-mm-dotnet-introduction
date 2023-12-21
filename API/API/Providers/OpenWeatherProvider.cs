using API.Models;

namespace API.Providers
{
    public class OpenWeatherProvider : IWeatherProvider
    {
        private string _name = "open_weather";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Weather? GetCurrentWeather()
        {
            var url = Environment.GetEnvironmentVariable("OPEN_WEATHER_URL");

            var lat = Environment.GetEnvironmentVariable("LAT");
            var lon = Environment.GetEnvironmentVariable("LON");
            var apikey = Environment.GetEnvironmentVariable("OPEN_WEATHER_API_KEY");

            var parameters = new[] {
                $"lat={lat}",
                $"lon={lon}",
                $"appid={apikey}",
                $"units=metric"
            };

            var requestUrl = url + string.Join("&", parameters);
            try
            {
                var response = new HttpClient().GetFromJsonAsync<ResponseModels.OpenWeather.Root>(requestUrl).Result;

                if (response == null)
                {
                    return null;
                }

                var weather = new Weather(
                        ServiceName: _name,
                        TemperatureC: response.main.temp.ToString(),
                        TemperatureF: Weather.ConvertFromCToF(response.main.temp).ToString(),
                        Cloud: Weather.NoInfoIfNull(response?.clouds?.all),
                        Humidity: Weather.NoInfoIfNull(response?.main?.humidity),
                        DirectionOfWind: Weather.NoInfoIfNull(response?.wind?.deg),
                        SpeedOfWind: Weather.NoInfoIfNull(response?.wind?.speed),
                        Precipitation: Weather.NoInfoIfNull(response?.snow?._1h)
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