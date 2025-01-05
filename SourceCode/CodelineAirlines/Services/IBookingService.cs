using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IBookingService
    {
        bool BookFlight(BookingDTO bookingDto);
        bool CancelBooking(int bookingId);
        int CancelFlightBookings(List<int> bookingsIds, string condition);
        IEnumerable<Booking> GetAllBookingsForAdmin();
        List<SeatsOutputDTO> GetAvailableSeats(int flightNo, string seatClass);
        IEnumerable<Booking> GetBookingsForPassenger(string passport);
        void RescheduledFlightBookings(List<int> bookingsIds, DateTime newDate);
        bool UpdateBooking(UpdateBookingDTO bookingDto);
    }
}