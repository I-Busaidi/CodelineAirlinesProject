namespace CodelineAirlines.DTOs.AirplaneDTOs
{
    public class AirplaneOutputDto
    {
        public int AirplaneId { get; set; }
        public string AirplaneModel { get; set; }
        public DateOnly ManufactureDate { get; set; }
        public int CurrentAirportId { get; set; }
        public string AirportName { get; set; }
        public bool IsActive { get; set; }
    }
}
