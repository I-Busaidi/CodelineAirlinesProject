using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.Enums;

namespace CodelineAirlines.DTOs.FlightDTOs
{
    public class FlightDetailedOutputDTO
    {
        public int FlightNo { get; set; }

        public string FlightStatus { get; set; }

        public AirportOutputDTO Source {  get; set; }
        
        public AirportOutputDTO Destination { get; set; }

        public string AirplaneModel { get; set; }

        public decimal Cost { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime? DepartureDate { get; set; }

        public DateTime? ArrivalDate { get; set; }

        public int BookingsCount { get; set; }

        public int BookedEconomySeatsCount { get; set; }

        public int BookedBusinessSeatsCount { get; set; }

        public int BookedFirstClassSeatsCount { get; set; }

        public decimal? Rating { get; set; }
    }
}
