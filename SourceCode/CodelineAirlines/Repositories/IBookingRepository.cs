using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IBookingRepository
    {
        void AddBooking(Booking booking);
    }
}