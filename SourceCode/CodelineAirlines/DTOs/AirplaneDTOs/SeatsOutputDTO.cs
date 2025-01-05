using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.AirplaneDTOs
{
    public class SeatsOutputDTO
    {
        public string Type { get; set; }

        public string SeatNumber { get; set; }

        public string SeatLocation { get; set; }

        public decimal SeatCost { get; set; }
    }
}
