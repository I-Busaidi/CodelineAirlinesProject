namespace CodelineAirlines.DTOs.BookingDTOs
{
    public class BookingDTO
    {
        public int FlightNo { get; set; }
        public string PassengerPassport { get; set; }
        public string SeatNo { get; set; }
        public string? Meal { get; set; }
        public int LoyaltyPointsToUse { get; set; }
    }
}
