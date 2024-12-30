using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public void UpdateBooking(Booking booking)
        {
            var existingBooking = _context.Bookings.FirstOrDefault(b => b.BookingId == booking.BookingId);
            if (existingBooking != null)
            {
                existingBooking.SeatNo = booking.SeatNo ?? existingBooking.SeatNo; // If SeatNo is provided, update it.
                existingBooking.Meal = booking.Meal ?? existingBooking.Meal; // If Meal is provided, update it.
                existingBooking.Status = booking.Status; // Update status
                existingBooking.TotalCost = booking.TotalCost; // Update cost if changed

                // Optionally, you can also update other fields if necessary

                _context.SaveChanges(); // Save the changes to the database
            }
            else
            {
                throw new Exception("Booking not found.");
            }
        }

        public Booking GetBookingById(int bookingId)
        {
            return _context.Bookings
                           .Include(b => b.Flight)
                           .Include(b => b.Passenger)
                           .FirstOrDefault(b => b.BookingId == bookingId);
        }

        public void CancelBooking(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking != null)
            {
                booking.Status = -1; // Assuming -1 indicates canceled status
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Booking not found.");
            }
        }
    }
}
