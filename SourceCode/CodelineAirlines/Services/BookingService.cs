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
        private readonly ISeatTemplateService _seatTemplateService;

        public BookingService(IFlightService flightService, IPassengerRepository passengerRepository, IBookingRepository bookingRepository, IEmailService emailService, ISeatTemplateService seatTemplateService)
        {
            _seatTemplateService = seatTemplateService;
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
            //var existingBooking = flight.Bookings.FirstOrDefault(b => b.Passenger.Passport == passenger.Passport);
            //if (existingBooking != null)
            //{
            //    throw new Exception("Passenger has already booked this flight.");
            //}

            // Check if the flight is fully booked
            int seatCapacity = _seatTemplateService.GetSeatTemplatesByModel(flight.Airplane.AirplaneModel).Count();
            if (flight.Bookings.Count >= seatCapacity)
            {
                throw new InvalidOperationException("This flight is fully booked");
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
            int bookingId = _bookingRepository.AddBooking(booking);

            // Add the booking to the flight's bookings collection (in-memory)
            flight.Bookings.Add(booking);

            // Add loyalty points to the passenger's account
            IncreaseLoyaltyPoints(passenger, flight.Cost);

            // Save the updated passenger with the new loyalty points
            _passengerRepository.UpdatePassenger(passenger);

            // Send booking confirmation email
            SendBookingConfirmationEmail(bookingId);  // Pass the booking object to the email method

            return true;
        }

        // Method to increase loyalty points based on flight cost (you can adjust the logic here)
        private void IncreaseLoyaltyPoints(Passenger passenger, decimal flightCost)
        {
            // For example, give 1 loyalty point per $100 spent on the flight
            int pointsEarned = (int)(flightCost / 100);
            passenger.LoyaltyPoints += pointsEarned;
        }


        // Method for Admin to get all bookings
        public IEnumerable<Booking> GetAllBookingsForAdmin()
        {
            // Call the repository to fetch all bookings
            return _bookingRepository.GetAllBookings();
        }

        // Method for Passenger to get their own bookings by passport
        public IEnumerable<Booking> GetBookingsForPassenger(string passport)
        {
            // Ensure that passport is provided for passenger
            if (string.IsNullOrEmpty(passport))
            {
                throw new Exception("Passport is required for passenger.");
            }

            // Call the repository to fetch bookings for a specific passenger
            return _bookingRepository.GetBookingsByPassenger(passport);
        }

        public bool UpdateBooking(UpdateBookingDTO bookingDto)
        {
            // Retrieve the existing booking by its ID
            var booking = _bookingRepository.GetBookingById(bookingDto.BookingId);
            if (booking == null)
            {
                throw new Exception("Booking not found.");
            }

            // Update the booking details
            booking.SeatNo = bookingDto.SeatNo ?? booking.SeatNo;
            booking.Meal = bookingDto.Meal ?? booking.Meal;

            // Call the repository to save the changes
            _bookingRepository.UpdateBooking(booking);

            // Send email about the update
            SendBookingUpdateEmail(booking.BookingId);  // Pass the booking object to the email method

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

            // Retrieve the associated flight for the booking
            if (booking.Flight == null)
            {
                throw new Exception("Flight not found.");
            }

            // Get the current time and compare with the flight's scheduled departure date
            var currentDate = DateTime.Now;
            var scheduledDepartureDate = booking.Flight.ScheduledDepartureDate;

            // Check if the cancellation is being attempted on the same day of departure
            if (scheduledDepartureDate.Date == currentDate.Date)
            {
                throw new Exception("Booking cannot be cancelled on the same day of departure.");
            }

            // Check if the flight has already departed (using ActualDepartureDate)
            if (booking.Flight.ActualDepartureDate.HasValue && booking.Flight.ActualDepartureDate.Value.Date <= currentDate.Date)
            {
                throw new Exception("Booking cannot be cancelled after the flight has departed.");
            }

            // Determine the refund percentage based on the time of cancellation
            double refundPercentage = GetRefundPercentage(scheduledDepartureDate, currentDate);

            // Call the repository to cancel the booking
            _bookingRepository.CancelBooking(bookingId);

            // Calculate the refund based on the percentage
            double refundAmount = (double)booking.Flight.Cost * refundPercentage;

            // Optionally, update the booking with the refund amount if needed
            _bookingRepository.UpdateRefundAmount(bookingId, refundAmount);

            // Send email about the cancellation and refund amount
            SendBookingCancellationEmail(booking, refundPercentage, refundAmount);

            return true;
        }

        private double GetRefundPercentage(DateTime departureDate, DateTime currentDate)
        {
            // Logic for determining the refund percentage based on the cancellation timing
            double refundPercentage = 0;

            TimeSpan timeUntilDeparture = departureDate - currentDate;

            if (timeUntilDeparture.TotalDays >= 30)
            {
                refundPercentage = 1.0; // Full refund if cancelled 30 or more days before departure
            }
            else if (timeUntilDeparture.TotalDays >= 14)
            {
                refundPercentage = 0.75; // 75% refund if cancelled 14-29 days before departure
            }
            else if (timeUntilDeparture.TotalDays >= 7)
            {
                refundPercentage = 0.50; // 50% refund if cancelled 7-13 days before departure
            }
            else if (timeUntilDeparture.TotalDays >= 1)
            {
                refundPercentage = 0.25; // 25% refund if cancelled 1-6 days before departure
            }
            else
            {
                refundPercentage = 0.0; // No refund if cancelled less than 24 hours before departure
            }

            return refundPercentage;
        }

        // Method to send booking confirmation email
        private void SendBookingConfirmationEmail(int bookingId)
        {
            var booking = _bookingRepository.GetBookingById(bookingId);
            string subject = "Booking Confirmation";
            string body = $"Dear {booking.Passenger.User.UserName}\n\n" +
                          $"Your booking for Flight {booking.FlightNo} has been confirmed.\n" +
                          $"Seat: {booking.SeatNo}\n" +
                          $"Meal: {booking.Meal}\n\n" +
                          $"Total Cost: ${booking.TotalCost}\n" +
                          $"We look forward to welcoming you aboard!\n" +
                          $"Thank you for choosing us!\n\n" +
                          $"Best regards,\nCodeline's Airline Team";
            _emailService.SendEmailAsync(booking.Passenger.User.UserEmail, subject, body);
        }

        // Method to send booking update email
        private void SendBookingUpdateEmail(int bookingId)
        {
            var booking = _bookingRepository.GetBookingById(bookingId);
            string subject = "Booking Update";
            string body = $"Dear {booking.Passenger.User.UserName},\n" +
                          $"Your booking for Flight {booking.FlightNo} has been updated.\n" +
                          $"New Seat: {booking.SeatNo}\n" +
                          $"New Meal: {booking.Meal}\n" +
                          $"New Total Cost: ${booking.TotalCost}\n" +
                          $"We look forward to welcoming you aboard!\n" +
                          $"Thank you for updating your booking with us.\n\n" +
                          $"Best regards,\nCodeline's Airline Team";
            _emailService.SendEmailAsync(booking.Passenger.User.UserEmail, subject, body);
        }

        // Method to send booking cancellation email
        private void SendBookingCancellationEmail(Booking booking, double refundPercentage, double refundAmount)
        {
            string subject = "Booking Cancellation";
            string body = $"Dear {booking.Passenger.User.UserName},\n" +
                          $"We regret to inform you that your booking for Flight {booking.FlightNo} has been canceled.\n" +
                          $"Seat: {booking.SeatNo}\n" +
                          $"Refund Percentage: {refundPercentage * 100}%. Refund Amount: {refundAmount}" +
                          $"We apologize for any inconvenience caused.\n\n" +
                          $"Best regards,\nCodeline's Airline Team";
            _emailService.SendEmailAsync(booking.Passenger.User.UserEmail, subject, body);
        }

        public int CancelFlightBookings(List<int> bookingsIds, string condition)
        {
            List<Booking> bookings = new List<Booking>();
            for (int i = 0; i < bookingsIds.Count; i++)
            {
                bookings.Add(_bookingRepository.GetBookingById(bookingsIds[i]));
            }
            // Call the repository to cancel the booking
            int bookingsCount = _bookingRepository.CancelBookingsRange(bookings);

            // Send email about the cancellation
            SendFlightBookingsCancellationEmail(bookings, condition);  // Pass the booking object to the email method

            return bookingsCount;
        }

        private void SendFlightBookingsCancellationEmail(List<Booking> bookings, string condition)
        {
            foreach (Booking booking in bookings)
            {
                string subject = $"Booking Cancellation Due to {condition}";
                string body = $"Dear {booking.Passenger.User.UserName},\n" +
                              $"We regret to inform you that your booking for Flight {booking.FlightNo} has been canceled due to {condition}.\n" +
                              $"Seat: {booking.SeatNo}\n" +
                              $"Total Cost Refund: ${booking.TotalCost}\n" +
                              $"We apologize for any inconvenience caused.";
                _emailService.SendEmailAsync(booking.Passenger.User.UserEmail, subject, body);
            }
        }
    }
}
