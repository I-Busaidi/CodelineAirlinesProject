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

        [HttpGet("GetWeatherForecast/{cityName}")]
        public async Task<IActionResult> GetWeatherForecast(string cityName = "Muscat")
        {
            try
            {
                var forecastData = await _weatherService.GetFiveDayForecastAsync(cityName);

                // Optional: Group data by day for easier display
                var groupedForecast = GroupForecastByDay(forecastData.List);

                return Ok(groupedForecast);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        private Dictionary<DateTime, List<ForecastItem>> GroupForecastByDay(List<ForecastItem> forecastItems)
        {
            var grouped = forecastItems
                .GroupBy(f => f.Dt.Date)  // Group by date only (ignores time)
                .ToDictionary(g => g.Key, g => g.ToList());

            return grouped;
        }
    }
}
