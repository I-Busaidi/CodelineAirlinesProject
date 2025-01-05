namespace CodelineAirlines.Helpers.WeatherForecast
{
    public class WeatherResponse
    {
        public Coordinates coord {  get; set; }
        public MainWeather Main { get; set; }
        public Weather[] Weather { get; set; }
        public Sys sys { get; set; }
        public string Name { get; set; }
        public string WeatherDescription => Weather != null && Weather.Length > 0
            ? Weather[0].Description
            : "No description available.";
    }
}
