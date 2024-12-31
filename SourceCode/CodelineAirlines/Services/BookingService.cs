using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;

namespace CodelineAirlines.Services
{
    public class BookingService : IBookingService
    {
        private readonly IFlightService _flightService;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmailService _emailService;

        public BookingService(IFlightService flightService, IPassengerRepository passengerRepository, IBookingRepository bookingRepository, IEmailService emailService)
        {
            _flightService = flightService;
            _passengerRepository = passengerRepository;
            _bookingRepository = bookingRepository;
            _emailService = emailService;
        }

        public bool BookFlight(BookingDTO bookingDto)
        {
            // Retrieve the flight and passenger synchronously
            var flight = _flightService.GetFlightByIdWithRelatedData(bookingDto.FlightNo);
            if (flight == null)
            {
                throw new Exception("Flight not found.");
            }

            var passenger = _passengerRepository.GetPassengerByPassport(bookingDto.PassengerPassport);
            if (passenger == null)
            {
                throw new Exception("Passenger not found.");
            }

            // Check for existing booking for the passenger
            var existingBooking = flight.Bookings.FirstOrDefault(b => b.Passenger.Passport == passenger.Passport);
            if (existingBooking != null)
            {
                throw new Exception("Passenger has already booked this flight.");
            }

            // Create new booking
            var booking = new Booking
            {
                FlightNo = bookingDto.FlightNo,
                PassengerPassport = bookingDto.PassengerPassport,
                SeatNo = bookingDto.SeatNo,
                Meal = bookingDto.Meal,
                TotalCost = flight.Cost,
                Status = 0,  // Assume booking is pending
                BookingDate = DateTime.Now,
                Passenger = passenger,
                Flight = flight
            };

            // Save the booking synchronously
            _bookingRepository.AddBooking(booking);

            // Add the booking to the flight's bookings collection (in-memory)
            flight.Bookings.Add(booking);

            // Optionally, mark the flight as fully booked if necessary
            if (flight.Bookings.Count >= 5)
            {
                flight.StatusCode = 1; // Mark flight as fully booked
            }

            return true;
        }

        public IEnumerable<Booking> GetBookings(string userRole, string userPassport = null)
        {
            // If the user is an admin, return all bookings
            if (userRole == "Admin")
            {
                return _bookingRepository.GetAllBookings();
            }

            // If the user is a passenger, return only their bookings
            if (userRole == "Passenger" && userPassport != null)
            {
                return _bookingRepository.GetBookingsByPassenger(userPassport);
            }

            throw new Exception("Invalid role or missing passport for passenger.");
        }

        public bool UpdateBooking(UpdateBookingDTO bookingDto)
        {
            // Retrieve the existing booking by its ID
            var booking = _bookingRepository.GetBookingById(bookingDto.BookingId);
            if (booking == null)
            {
                throw new Exception("Booking not found.");
            }

            // Optionally, you can add validation to make sure the booking is not already canceled or completed, etc.

            // Update the booking details
            booking.SeatNo = bookingDto.SeatNo ?? booking.SeatNo;
            booking.Meal = bookingDto.Meal ?? booking.Meal;
            booking.Status = bookingDto.Status;  // Update the status if provided
            booking.TotalCost = bookingDto.TotalCost; // Update cost if changed

            // Call the repository to save the changes
            _bookingRepository.UpdateBooking(booking);

            return true; // Return true if update is successful
        }

        public bool CancelBooking(int bookingId)
        {
            // Retrieve the existing booking by ID
            var booking = _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                throw new Exception("Booking not found.");
            }

            // Check if the booking is already canceled
            if (booking.Status == -1)
            {
                throw new Exception("Booking is already canceled.");
            }

            // Call the repository to cancel the booking
            _bookingRepository.CancelBooking(bookingId);

            return true;
        }

        // Method to send booking confirmation email
        private void SendBookingConfirmationEmail(Booking booking)
        {
            string subject = "Booking Confirmation";
            string body = $"Dear {booking.User.UserName},<br><br>" +
                          $"Your booking for Flight {booking.FlightNo} has been confirmed.<br>" +
                          $"Seat: {booking.SeatNo}<br>" +
                          $"Total Cost: ${booking.TotalCost}<br>" +
                          $"We look forward to welcoming you aboard!<br><br>" +
                          $"Thank you for choosing us!";
            _emailService.SendEmailAsync(booking.User.UserEmail, subject, body);
        }

        // Method to send booking update email
        private void SendBookingUpdateEmail(Booking booking)
        {
            string subject = "Booking Update";
            string body = $"Dear {booking.User.UserName},<br><br>" +
                          $"Your booking for Flight {booking.FlightNo} has been updated.<br>" +
                          $"New Seat: {booking.SeatNo}<br>" +
                          $"New Meal: {booking.Meal}<br>" +
                          $"New Total Cost: ${booking.TotalCost}<br>" +
                          $"Thank you for updating your booking with us.";
            _emailService.SendEmailAsync(booking.User.UserEmail, subject, body);
        }

        // Method to send booking cancellation email
        private void SendBookingCancellationEmail(Booking booking)
        {
            string subject = "Booking Cancellation";
            string body = $"Dear {booking.User.UserName},<br><br>" +
                          $"We regret to inform you that your booking for Flight {booking.FlightNo} has been canceled.<br>" +
                          $"Seat: {booking.SeatNo}<br>" +
                          $"Total Cost: ${booking.TotalCost}<br>" +
                          $"We apologize for any inconvenience caused.";
            _emailService.SendEmailAsync(booking.User.UserEmail, subject, body);
        }
    }
}
