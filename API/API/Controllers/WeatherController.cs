using Microsoft.AspNetCore.Mvc;
using API.Providers;

namespace API.Controllers
{

    [ApiController]
    [Route("/weather")]
    public class WeatherController : ControllerBase
    {
        IEnumerable<IWeatherProvider> _providers;

        public WeatherController(IEnumerable<IWeatherProvider> providers)
        {
            _providers = providers;
        }

        [HttpGet]
        [Route("/providers")]
        public IActionResult GetProviders()
        {
            return Ok(_providers.ToList());
        }

        [HttpGet]
        public IActionResult GetWeatherFromAllSources()
        {
            var weathers = from provider in _providers select provider.GetCurrentWeather();
            return Ok(weathers);
        }

        [HttpGet]
        [Route("/{providerName}")]
        [ProducesResponseType(typeof(Models.Weather), StatusCodes.Status200OK)]
        public IActionResult GetWeatherFromProvider(string providerName)
        {
            var foundProvider = _providers.FirstOrDefault((provider) => provider.Name.Equals(providerName), null);

            if(foundProvider == null) return NotFound("Provider not found");

            var weather = foundProvider.GetCurrentWeather();
            if(weather == null) return NotFound("Given provider didn`t respond");

            return Ok(weather);
        }
    }
}