using CodelineAirlines.Helpers.WeatherForecast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class WeatherController : ControllerBase
    { 
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("GetCurrentWeather/{cityName}")]
        public async Task<IActionResult> GetWeather(string cityName = "Muscat")
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherAsync(cityName);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
