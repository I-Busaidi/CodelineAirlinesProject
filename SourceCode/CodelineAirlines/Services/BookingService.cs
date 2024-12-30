using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;

namespace CodelineAirlines.Services
{
    public class BookingService : IBookingService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IFlightRepository flightRepository, IPassengerRepository passengerRepository, IBookingRepository bookingRepository)
        {
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
            _bookingRepository = bookingRepository;
        }

        public bool BookFlight(BookingDTO bookingDto)
        {
            // Retrieve the flight and passenger synchronously
            var flight = _flightRepository.GetFlightByNo(bookingDto.FlightNo);
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
            var existingBooking = flight.Bookings.FirstOrDefault(b => b.Passenger.PassengerPassport == passenger.Passport);
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
            if (flight.Bookings.Count >= flight.Airplane.Capacity)
            {
                flight.StatusCode = 1; // Mark flight as fully booked
            }

            return true;
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
    }
}
