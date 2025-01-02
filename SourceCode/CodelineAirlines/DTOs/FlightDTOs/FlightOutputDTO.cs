using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.FlightDTOs
{
    public class FlightOutputDTO
    {
        public int FlightNo { get; set; }

        public string SourceAirportName { get; set; }

        public string DestinationAirportName { get; set; }

        public int AirplaneId { get; set; }

        public string AirplaneModel { get; set; }

        public string Status { get; set; }

        public decimal Cost { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime ScheduledDepartureDate { get; set; }

        public DateTime ScheduledArrivalDate { get; set; }
    }
}
