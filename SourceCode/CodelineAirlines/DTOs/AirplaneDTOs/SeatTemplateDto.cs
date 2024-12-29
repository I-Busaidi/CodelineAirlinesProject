namespace CodelineAirlines.DTOs.AirplaneDTOs
{
    public class GenerateSeatTemplateDto
    {
        public string AirplaneModel { get; set; }
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
    }
}
