namespace CodelineAirlines.Helpers.WeatherForecast
{
    public class ForecastItem
    {
        public DateTime Dt { get; set; }  // Date and time of forecast
        public MainWeather Main { get; set; }
        public Weather[] Weather { get; set; }
    }
}
