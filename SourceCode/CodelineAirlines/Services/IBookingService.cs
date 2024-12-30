using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IBookingService
    {
        bool BookFlight(BookingDTO bookingDto);
        bool CancelBooking(int bookingId);
        IEnumerable<Booking> GetBookings(string userRole, string userPassport = null);
        bool UpdateBooking(UpdateBookingDTO bookingDto);
    }
}