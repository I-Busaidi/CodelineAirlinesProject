using CodelineAirlines.DTOs.AirplaneDTOs;
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

        public int AddBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return booking.BookingId;
        }

        public IEnumerable<Booking> GetAllBookings()
        {
            return _context.Bookings
                           .Include(b => b.Flight)
                           .Include(b => b.Passenger)
                           .ThenInclude(b => b.User)
                           .ToList();
        }

        public IEnumerable<Booking> GetBookingsByPassenger(string passengerPassport)
        {
            return _context.Bookings
                           .Include(b => b.Flight)
                           .Include(b => b.Passenger)
                           .ThenInclude(b => b.User)
                           .Where(b => b.Passenger.Passport == passengerPassport)
                           .ToList();
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
                           .ThenInclude(b => b.User)
                           .FirstOrDefault(b => b.BookingId == bookingId);
        }

        public void CancelBooking(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Booking not found.");
            }
        }

        public int CancelBookingsRange(List<Booking> bookings)
        {
            _context.Bookings.RemoveRange(bookings);
            return bookings.Count();
        }
    }
}
