using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IBookingRepository
    {
        void AddBooking(Booking booking);
        Booking GetBookingById(int bookingId);
        void UpdateBooking(Booking booking);
    }
}