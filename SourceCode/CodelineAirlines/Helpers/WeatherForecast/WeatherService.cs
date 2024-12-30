using Newtonsoft.Json;

namespace CodelineAirlines.Helpers.WeatherForecast
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "974e95acb2b9b479fdbdacf4d68272cd";
        private const string BaseUrl = "http://api.openweathermap.org/data/2.5/weather";

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherResponse> GetWeatherAsync(string cityName)
        {
            var url = $"{BaseUrl}?q={cityName}&appid={ApiKey}&units=metric"; // Adjust units if needed (metric, imperial, etc.)
            var response = await _httpClient.GetStringAsync(url);
            var weatherData = JsonConvert.DeserializeObject<WeatherResponse>(response);
            return weatherData;
        }

        public async Task<ForecastResponse> GetFiveDayForecastAsync(string cityName)
        {
            var url = $"{BaseUrl}?q={cityName}&appid={ApiKey}&units=metric";
            var response = await _httpClient.GetStringAsync(url);
            var forecastData = JsonConvert.DeserializeObject<ForecastResponse>(response);
            return forecastData;
        }
    }
}
