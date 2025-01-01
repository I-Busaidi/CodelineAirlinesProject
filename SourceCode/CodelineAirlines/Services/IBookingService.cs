using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IBookingService
    {
        bool BookFlight(BookingDTO bookingDto);
        bool CancelBooking(int bookingId);
        IEnumerable<Booking> GetAllBookingsForAdmin();
        IEnumerable<Booking> GetBookingsForPassenger(string passport);
        bool UpdateBooking(UpdateBookingDTO bookingDto);
        int CancelFlightBookings(List<int> bookingsIds, string condition);
    }
}