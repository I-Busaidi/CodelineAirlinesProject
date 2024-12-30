namespace CodelineAirlines.DTOs.BookingDTOs
{
    public class UpdateBookingDTO
    {
        public int BookingId { get; set; }   // The BookingId to identify which booking to update
        public string? SeatNo { get; set; }   // Optional: new seat number
        public string? Meal { get; set; }     // Optional: new meal preference
        public int Status { get; set; }       // New status of the booking (e.g., pending, confirmed, canceled)
        public decimal TotalCost { get; set; } // New total cost (if updated)
    }
}
