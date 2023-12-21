using API.Models;

namespace API.Providers
{
    public interface IWeatherProvider
    {
        public string Name { get; }

        public Weather? GetCurrentWeather();
    }
}