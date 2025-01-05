namespace CodelineAirlines.DTOs.BookingDTOs
{
    public class UpdateBookingDTO
    {
        public int BookingId { get; set; }   // The BookingId to identify which booking to update
        public string? Class {  get; set; } // // Optional: new class selection
        public string? SeatNo { get; set; }   // Optional: new seat number
        public string? Meal { get; set; }     // Optional: new meal preference
    }
}
