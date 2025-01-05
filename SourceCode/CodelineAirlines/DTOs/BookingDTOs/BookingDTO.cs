using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        [Required]
        public int FlightNo { get; set; }
        [Required]
        public string PassengerPassport { get; set; }
        [Required]
        public string Class { get; set; }
        public string? SeatNo { get; set; }
        public string? Meal { get; set; }
        public int LoyaltyPointsToUse { get; set; }
    }
}
