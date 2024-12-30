namespace CodelineAirlines.Helpers.WeatherForecast
{
    public class ForecastResponse
    {
        public CityInfo City { get; set; }
        public List<ForecastItem> List { get; set; }
    }
}
