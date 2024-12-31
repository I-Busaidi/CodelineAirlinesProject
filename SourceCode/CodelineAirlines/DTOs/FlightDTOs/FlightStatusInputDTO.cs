using CodelineAirlines.Enums;

namespace CodelineAirlines.DTOs.FlightDTOs
{
    public class FlightStatusInputDTO
    {
        public int FlightNo { get; set; }
        public FlightStatus FlightStatus { get; set; }
    }
}
