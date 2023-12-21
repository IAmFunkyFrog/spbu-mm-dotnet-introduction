namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("TOMORROW_IO_URL", "https://api.tomorrow.io/v4/timelines?");
        Environment.SetEnvironmentVariable("LAT", "59.93863");
        Environment.SetEnvironmentVariable("LON", "30.31413");
        Environment.SetEnvironmentVariable("TOMORROW_IO_API_KEY", "944Ubw2jFtqMUZnVNXWInCkezgNumRK2");
        Environment.SetEnvironmentVariable("OPEN_WEATHER_URL", "https://api.openweathermap.org/data/2.5/weather?");
        Environment.SetEnvironmentVariable("OPEN_WEATHER_API_KEY", "3f314355f307b0b22f532bc5aaf238e2");
    }

    [Test]
    public void TestTomorrowIOReturnsNotNull()
    {
        API.Providers.IWeatherProvider service = new API.Providers.TomorrowIOProvider();
        API.Models.Weather? response = service.GetCurrentWeather();
        Assert.That(response != null);
    }

    [Test]
    public void TestOpenWeatherReturnsNotNull()
    {
        API.Providers.IWeatherProvider service = new API.Providers.OpenWeatherProvider();
        API.Models.Weather? response = service.GetCurrentWeather();
        Assert.That(response != null);
    }
}