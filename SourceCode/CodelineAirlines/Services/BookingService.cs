using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Enums;
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
        private readonly IMapper _mapper;


        public BookingService(IFlightService flightService, IPassengerRepository passengerRepository, IBookingRepository bookingRepository, IEmailService emailService, ISeatTemplateService seatTemplateService, IMapper mapper)
        {
            _seatTemplateService = seatTemplateService;
            _flightService = flightService;
            _passengerRepository = passengerRepository;
            _bookingRepository = bookingRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public bool BookFlight(BookingDTO bookingDto)
        {
            // Define class cost percentages
            const decimal EconomyClassPercentage = 0.00m; // 0% for Economy
            const decimal BusinessClassPercentage = 0.05m; // 5% for Business
            const decimal FirstClassPercentage = 0.10m; // 10% for First Class
            const decimal SeatSelectionPercentage = 0.03m; // 3% for seat selection

            // Retrieve the flight and passenger synchronously
            var flight = _flightService.GetFlightByIdWithRelatedData(bookingDto.FlightNo);
            if (flight == null)
            {
                throw new Exception("Flight not found.");
            }

            // Validate flight status
            if (!Enum.IsDefined(typeof(FlightStatus), flight.StatusCode))
            {
                throw new ArgumentOutOfRangeException(nameof(flight.StatusCode), "Invalid flight status code.");
            }

            if (flight.StatusCode != (int)FlightStatus.Scheduled && flight.StatusCode != (int)FlightStatus.ReScheduled)
            {
                throw new InvalidOperationException("Booking can only be added for flights that are scheduled or rescheduled.");
            }

            var passenger = _passengerRepository.GetPassengerByPassport(bookingDto.PassengerPassport);
            if (passenger == null)
            {
                throw new Exception("Passenger not found.");
            }

            // Check if the passenger has enough loyalty points
            if (bookingDto.LoyaltyPointsToUse > passenger.LoyaltyPoints)
            {
                throw new InvalidOperationException("Insufficient loyalty points.");
            }

            // Get available seats for the selected class
            var availableSeats = GetAvailableSeats(bookingDto.FlightNo, bookingDto.Class);

            // Check if the selected seat is available
            if (!string.IsNullOrEmpty(bookingDto.SeatNo) && !availableSeats.Any(s => s.SeatNumber == bookingDto.SeatNo))
            {
                throw new InvalidOperationException("The selected seat is not available.");
            }

            var seats = _seatTemplateService.GetSeatTemplatesByModel(flight.Airplane.AirplaneModel);

            // Check if the flight is fully booked
            int seatCapacity = seats.Count();
            if (flight.Bookings.Count >= seatCapacity)
            {
                throw new InvalidOperationException("This flight is fully booked");
            }

            // Determine the base cost of the flight
            decimal baseCost = flight.Cost; // Use the flight's base cost

            // Calculate the class cost based on the selected class
            decimal classCost = 0;
            switch (bookingDto.Class.ToLower())
            {
                case "economy":
                    classCost = baseCost * EconomyClassPercentage;
                    break;
                case "business":
                    classCost = baseCost * BusinessClassPercentage;
                    break;
                case "first":
                    classCost = baseCost * FirstClassPercentage;
                    break;
                default:
                    throw new ArgumentException("Invalid class selected.");
            }

            // Calculate the total cost
            decimal totalCost = baseCost + classCost;

            // If a seat is selected, add the seat selection cost
            if (!string.IsNullOrEmpty(bookingDto.SeatNo) && seats.Any(s => s.SeatNumber == bookingDto.SeatNo))
            {
                totalCost += baseCost * SeatSelectionPercentage; // Add 3% of the base cost
            }
            else
            {
                bookingDto.SeatNo = null;
            }

            if (string.IsNullOrEmpty(bookingDto.Meal) || bookingDto.Meal.ToLower() == "string")
            {
                bookingDto.Meal = null;
            }

            // Calculate the discount based on loyalty points used
            decimal loyaltyPointsValue = bookingDto.LoyaltyPointsToUse * 10m;  // Assuming 1 point = $10 discount
            if (loyaltyPointsValue > totalCost)
            {
                loyaltyPointsValue = totalCost;  // Ensure the discount does not exceed the total cost
            }

            // Apply the discount to the total cost
            totalCost -= loyaltyPointsValue;

            // Create new booking with the discounted total cost
            var booking = new Booking
            {
                FlightNo = bookingDto.FlightNo,
                PassengerPassport = bookingDto.PassengerPassport,
                Class = bookingDto.Class,
                SeatNo = bookingDto.SeatNo,
                Meal = bookingDto.Meal,
                TotalCost = totalCost,  // Apply the total cost here
                Status = 0,  // Assume booking is pending
                BookingDate = DateTime.Now,
                LoyaltyPointsUsed = bookingDto.LoyaltyPointsToUse,
                Passenger = passenger,
                Flight = flight
            };

            // Save the booking synchronously
            int bookingId = _bookingRepository.AddBooking(booking);

            // Add the booking to the flight's bookings collection (in-memory)
            flight.Bookings.Add(booking);

            // Deduct the loyalty points used from the passenger's account
            passenger.LoyaltyPoints -= bookingDto.LoyaltyPointsToUse;

            // Save the updated passenger with the reduced loyalty points
            _passengerRepository.UpdatePassenger(passenger);

            // Send booking confirmation email with loyalty points and discount details
            SendBookingConfirmationEmail(bookingId, bookingDto.LoyaltyPointsToUse, loyaltyPointsValue);

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
            booking.Class = bookingDto.Class ?? booking.Class;
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
                // If booking is not found, show a message and return false
                throw new Exception("Booking not found.");
            }

            // Retrieve the associated flight for the booking
            if (booking.Flight == null)
            {
                // If flight is not found, handle this as an error
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

            // Call the repository to cancel the booking (this deletes the booking)
            _bookingRepository.CancelBooking(bookingId);

            // Calculate the refund amount based on the refund percentage
            double refundAmount = (double)booking.Flight.Cost * refundPercentage;

            // Refund the loyalty points used for this booking
            int pointsRefunded = booking.LoyaltyPointsUsed;

            // Add the refunded loyalty points back to the passenger's account
            var passenger = booking.Passenger;
            passenger.LoyaltyPoints += pointsRefunded;

            // Update the passenger's loyalty points in the repository
            _passengerRepository.UpdatePassenger(passenger);

            // Send email about the cancellation, refund amount, and refunded loyalty points
            SendBookingCancellationEmail(booking, refundPercentage, refundAmount, pointsRefunded);

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
        private void SendBookingConfirmationEmail(int bookingId, int loyaltyPointsUsed, decimal discountAmount)
        {
            var booking = _bookingRepository.GetBookingById(bookingId);
            string subject = "Booking Confirmation";
            string body = $"Dear {booking.Passenger.User.UserName}\n\n" +
                          $"Your booking for Flight {booking.FlightNo} has been confirmed.\n" +
                          $"On {booking.Flight.ScheduledDepartureDate.ToString("dddd ~ dd/MM/yyyy ~ hh:mm:ss tt")}\n" +
                          $"Class: {booking.Class}\n";
            if (booking.SeatNo != null)
            {
                body += $"Seat: {booking.SeatNo}\n";
            }

            if (booking.Meal != null)
            {
                body += $"Meal: {booking.Meal}\n\n";
            }

            // Include loyalty points and discount information
            if (loyaltyPointsUsed > 0)
            {
                body += $"Loyalty Points Used: {loyaltyPointsUsed} points\n" +
                        $"Discount Applied: ${discountAmount}\n\n";
            }

            body += $"Total Cost: ${booking.TotalCost}\n\n" +
                    $"We look forward to welcoming you aboard!\n" +
                    $"Thank you for choosing us!\n\n" +
                    $"Best regards,\nCodeline's Airline Team";

            // Send the email
            _emailService.SendEmailAsync(booking.Passenger.User.UserEmail, subject, body);
        }


        // Method to send booking update email
        private void SendBookingUpdateEmail(int bookingId)
        {
            var booking = _bookingRepository.GetBookingById(bookingId);
            string subject = "Booking Update";
            string body = $"Dear {booking.Passenger.User.UserName},\n\n" +
                          $"Your booking for Flight {booking.FlightNo} has been updated.\n" +
                          $"Date: {booking.Flight.ScheduledDepartureDate.ToString("dddd ~ dd/MM/yyyy ~ hh:mm:ss tt")}\n" + 
                          $"New Class: {booking.Class}\n" +
                          $"New Seat: {booking.SeatNo}\n" +
                          $"New Meal: {booking.Meal}\n" +
                          $"New Total Cost: ${booking.TotalCost}\n\n" +
                          $"We look forward to welcoming you aboard!\n" +
                          $"Thank you for updating your booking with us.\n\n" +
                          $"Best regards,\nCodeline's Airline Team";
            _emailService.SendEmailAsync(booking.Passenger.User.UserEmail, subject, body);
        }

        // Method to send booking cancellation email
        private void SendBookingCancellationEmail(Booking booking, double refundPercentage, double refundAmount, int pointsRefunded)
        {
            string subject = "Booking Cancellation Confirmation";
            string body = $"Dear {booking.Passenger.User.UserName},\n\n" +
                          $"We regret to inform you that your booking for Flight {booking.FlightNo} has been cancelled.\n";
            if (booking.SeatNo != null)
            {
                body += $"Seat: {booking.SeatNo}\n";
            }
            body += $"Refund Percentage: {refundPercentage * 100}%\n" +
                          $"Refund Amount: ${refundAmount}\n\n";

            // Include refunded loyalty points
            if (pointsRefunded > 0)
            {
                body += $"Loyalty Points Refunded: {pointsRefunded} points\n\n";
            }

            body += $"We hope to welcome you on board in the future!\n" +
                    $"Thank you for choosing us!\n\n" +
                    $"Best regards,\nCodeline's Airline Team";

            // Send the email
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

        public void RescheduledFlightBookings(List<int> bookingsIds, DateTime newDate)
        {
            List<Booking> bookings = new List<Booking>();
            for (int i = 0; i < bookingsIds.Count; i++)
            {
                bookings.Add(_bookingRepository.GetBookingById(bookingsIds[i]));
            }
            // Send email about the cancellation
            SendFlightBookingsRescheduleEmail(bookings, newDate);  // Pass the booking object to the email method
        }

        private void SendFlightBookingsRescheduleEmail(List<Booking> bookings, DateTime newDate)
        {
            foreach (Booking booking in bookings)
            {
                string subject = $"Booked Flight Rescheduled";
                string body = $"Dear {booking.Passenger.User.UserName},\n" +
                              $"We regret to inform you that your booked flight {booking.FlightNo} has been rescheduled to {newDate.ToString("dd/MM/yyyy ddd ~ hh:mm:ss tt")}.\n" +
                              $"We apologize for any inconvenience caused.";
                _emailService.SendEmailAsync(booking.Passenger.User.UserEmail, subject, body);
            }
        }

        public List<SeatsOutputDTO> GetAvailableSeats(int flightNo, string seatClass)
        {
            var flight = _flightService.GetFlightByIdWithRelatedData(flightNo);
            var seats = _seatTemplateService.GetSeatTemplatesByModel(flight.Airplane.AirplaneModel);

            if (seats == null || seats.Count() == 0)
            {
                throw new KeyNotFoundException("Could not find seat.");
            }

            // Filter seats based on the selected class
            var availableSeats = seats
                .Where(s => NormalizeString(s.Type).Contains(NormalizeString(seatClass)) &&
                            !flight.Bookings.Any(b => b.SeatNo == s.SeatNumber))
                .ToList();

            List<SeatsOutputDTO> availableSeatsList = _mapper.Map<List<SeatsOutputDTO>>(availableSeats);
            return availableSeatsList;
        }
        private string NormalizeString(string input)
        {
            return string.Concat(input.Where(c => !char.IsWhiteSpace(c))).ToLower();
        }
    }
}
