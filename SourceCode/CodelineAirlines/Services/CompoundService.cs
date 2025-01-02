using ActiveUp.Net.Mail;
using AutoMapper;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.Enums;
using CodelineAirlines.Models;
using MimeKit.Encodings;

namespace CodelineAirlines.Services
{
    public class CompoundService : ICompoundService
    {
        private readonly IFlightService _flightService;
        private readonly IAirportService _airportService;
        private readonly IAirplaneService _airplaneService;
        private readonly IBookingService _bookingService;
        private readonly ISeatTemplateService _seatTemplateService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IReviewService _reviewService;
        private readonly IPassengerService _passengerService;


        public CompoundService(IFlightService flightService, IAirportService airportService, IAirplaneService airplaneService, IMapper mapper, IBookingService bookingService, ApplicationDbContext context, ISeatTemplateService seatTemplateService, IReviewService reviewService,IPassengerService passengerService)
        {
            _context = context;
            _bookingService = bookingService;
            _flightService = flightService;
            _airportService = airportService;
            _airplaneService = airplaneService;
            _seatTemplateService = seatTemplateService;
            _reviewService = reviewService;
            _mapper = mapper;
            _passengerService = passengerService;
          }

        public int AddFlight(FlightInputDTO flightInput)
        {
            if (flightInput.Cost <= 0)
            {
                throw new ArgumentException("Flight cost must be greater than 0");
            }

            var airplane = _airplaneService.GetAirplaneByIdWithRelatedData(flightInput.AirplaneId);
            if (airplane == null)
            {
                throw new KeyNotFoundException("Airplane not found");
            }

            var sourceAirport = _airportService.GetAirportByNameWithRelatedData(flightInput.SourceAirportName);
            if (sourceAirport == null)
            {
                throw new KeyNotFoundException("Source airport not found");
            }

            var destinationAirport = _airportService.GetAirportByNameWithRelatedData(flightInput.DestinationAirportName);
            if (destinationAirport == null)
            {
                throw new KeyNotFoundException("Destination airport not found");
            }

            if (!CheckAirplaneAvailability(airplane, sourceAirport, flightInput))
            {
                throw new InvalidOperationException("Airplane is not available for this flight");
            }

            var flight = _mapper.Map<Flight>(flightInput);
            return _flightService.AddFlight(flight);
        }

        public (int FlightNo, DateTime NewDepartureDate) RescheduleFlight(int flightNo, DateTime newDate, int airplaneId = -1)
        {
            var flight = _flightService.GetFlightByIdWithRelatedData(flightNo);
            if (flight == null)
            {
                throw new KeyNotFoundException("Could not find flight");
            }

            if (newDate <= DateTime.Now.AddDays(3))
            {
                throw new InvalidOperationException("Cannot reschedule a flight 3 days before the flight departure date");
            }

            flight.ScheduledDepartureDate = newDate;

            if (airplaneId == -1)
            {
                airplaneId = flight.AirplaneId;
            }
            else
            {
                flight.AirplaneId = airplaneId;
            }

            var airplane = _airplaneService.GetAirplaneByIdWithRelatedData(flight.AirplaneId);
            if (airplane == null)
            {
                throw new KeyNotFoundException("Airplane not found");
            }

            if (!CheckAirplaneAvailabilityForReschedule(flight))
            {
                throw new InvalidOperationException("Airplane is not available for this flight");
            }

            using (var transcation = _context.Database.BeginTransaction())
            {
                try
                {
                    flight.StatusCode = 6;
                    int updatedFlightNo = _flightService.CancelFlight(flight);

                    var bookings = flight.Bookings.Select(b => b.BookingId).ToList();

                    if (bookings != null || bookings.Count > 0)
                    {
                        _bookingService.RescheduledFlightBookings(bookings, newDate);
                    }

                    _context.SaveChanges();
                    transcation.Commit();

                    return (flightNo, newDate);
                }
                catch (Exception ex)
                {
                    transcation.Rollback();
                    throw new InvalidOperationException("An error occured when canceling flight");
                }
            }
        }

        public (int, string) UpdateFlightStatus(int flightId, FlightStatus flightStatus)
        {
            var flight = _flightService.GetFlightByIdWithRelatedData(flightId);
            if (flight == null)
            {
                throw new KeyNotFoundException("Could not find flight");
            }

            flight.StatusCode = (int)flightStatus;

            _flightService.UpdateFlightStatus(flight);
            return (flight.FlightNo, flightStatus.ToString());
        }

        public (int flightNo, int BookingsCount) CancelFlight(int flightId, string condition)
        {
            var flight = _flightService.GetFlightByIdWithRelatedData(flightId);
            if (flight == null)
            {
                throw new KeyNotFoundException("Flight not found");
            }

            if (flight.StatusCode == 5)
            {
                throw new InvalidOperationException("This flight has already been canceled");
            }

            if (flight.StatusCode == 4)
            {
                throw new InvalidOperationException("This flight has already arrived to the destination");
            }

            if (flight.StatusCode > 0 && flight.StatusCode < 4)
            {
                throw new InvalidOperationException("This flight has already taken off and cannot be canceled");
            }

            // begin transaction
            using (var transcation = _context.Database.BeginTransaction())
            {
                try
                {
                    flight.StatusCode = 5;
                    int flightNo = _flightService.CancelFlight(flight);

                    var bookings = flight.Bookings.Select(b => b.BookingId).ToList();
                    int bookingsCount = 0;

                    if (bookings != null || bookings.Count > 0)
                    {

                        bookingsCount = _bookingService.CancelFlightBookings(bookings, condition);
                    }

                    _context.SaveChanges();
                    transcation.Commit();

                    return (flightNo, bookingsCount);
                }
                catch (Exception ex) 
                {
                    transcation.Rollback();
                    throw new InvalidOperationException("An error occured when canceling flight");
                }
            }
        }

        public FlightDetailedOutputDTO GetFlightDetails(int flightNo)
        {
            var flight = _flightService.GetFlightByIdWithRelatedData(flightNo);
            var seatTemplate = _seatTemplateService.GetSeatTemplatesByModel(flight.Airplane.AirplaneModel).ToList();
            DateTime? departureDate;
            DateTime? arrivalDate;
            int bookingsCount = 0;
            int bookedEconomy = 0;
            int bookedBusiness = 0;
            int bookedFirst = 0;
            decimal rating = 0;

            if (flight.FlightRating != null)
            {
                rating = flight.FlightRating.Value;
            }

            if (flight.Bookings != null || flight.Bookings.Count > 0)
            {
                bookingsCount = flight.Bookings.Count;
                foreach (var booking in flight.Bookings)
                {
                    foreach (var seat in seatTemplate)
                    {
                        if (booking.SeatNo == seat.SeatNumber)
                        {
                            if (seat.Type == "Economy")
                            {
                                bookedEconomy++;
                            }
                            if (seat.Type == "Business")
                            {
                                bookedBusiness++;
                            }
                            if (seat.Type == "First Class")
                            {
                                bookedFirst++;
                            }
                        }
                    }
                }
            }


            if (flight.ActualDepartureDate != null)
            {
                departureDate = flight.ActualDepartureDate;
            }
            else
            {
                departureDate = flight.ScheduledDepartureDate;
            }

            if (flight.EstimatedArrivalDate != null)
            {
                arrivalDate = flight.EstimatedArrivalDate;
            }
            else
            {
                arrivalDate = flight.ScheduledArrivalDate;
            }

            FlightDetailedOutputDTO flightDetails = new FlightDetailedOutputDTO
            {
                FlightNo = flight.FlightNo,
                Source = _mapper.Map<AirportOutputDTO>(flight.SourceAirport),
                Destination = _mapper.Map<AirportOutputDTO>(flight.DestinationAirport),
                FlightStatus = Enum.GetName(typeof(FlightStatus), flight.StatusCode),
                AirplaneModel = flight.Airplane.AirplaneModel,
                Cost = flight.Cost,
                Duration = flight.Duration,
                DepartureDate = departureDate,
                ArrivalDate = arrivalDate,
                BookingsCount = flight.Bookings.Count,
                BookedEconomySeatsCount = bookedEconomy,
                BookedBusinessSeatsCount = bookedBusiness,
                BookedFirstClassSeatsCount = bookedFirst,
                Rating = rating
            };

            return flightDetails;
        }

        private bool CheckAirplaneAvailabilityForReschedule(Flight flightInput)
        {
            var priorFlight = _flightService.GetPriorFlightForReschedule(flightInput.Airplane.AirplaneId, flightInput.FlightNo);
            if (priorFlight != null)
            {
                if (priorFlight.DestinationAirportId != flightInput.SourceAirport.AirportId || priorFlight.ScheduledArrivalDate >= flightInput.ScheduledDepartureDate)
                {
                    return false;
                }
            }
            else
            {
                if (flightInput.Airplane.CurrentAirportId != flightInput.SourceAirport.AirportId)
                {
                    return false;
                }
            }

            if (_flightService.IsFlightConflictingForReschedule(flightInput))
            {
                return false;
            }

            return true;
        }

        private bool CheckAirplaneAvailability(Airplane airplane, Airport srcAirport, FlightInputDTO flightInput)
        {
            var priorFlight = _flightService.GetPriorFlight(airplane.AirplaneId);
            if (priorFlight != null)
            {
                if (priorFlight.DestinationAirportId != srcAirport.AirportId || priorFlight.ScheduledArrivalDate >= flightInput.ScheduledDepartureDate)
                {
                    return false;
                }
            }
            else
            {
                if (airplane.CurrentAirportId != srcAirport.AirportId)
                {
                    return false;
                }
            }

            if (_flightService.IsFlightConflicting(flightInput))
            {
                return false;
            }

            return true;
        }
        public void AddReview(ReviewInputDTO review)
        {
            // Retrieve the flight details
            var flight = _flightService.GetFlightByIdWithRelatedData(review.FlightNo);
            if (flight == null)
            {
                throw new KeyNotFoundException($"Flight with ID {review.FlightNo} not found.");
            }

            // Validate flight status
            if (!Enum.IsDefined(typeof(FlightStatus), flight.StatusCode))
            {
                throw new ArgumentOutOfRangeException(nameof(flight.StatusCode), "Invalid flight status code.");
            }

            if ((FlightStatus)flight.StatusCode != FlightStatus.Arrived)
            {
                throw new InvalidOperationException("Reviews can only be added for flights that have arrived.");
            }
            // Retrieve the reviewer (passenger) details
            var reviewer = _passengerService.GetPassengerByPassport(review.ReviewerPassport);
            if (reviewer == null)
            {
                throw new KeyNotFoundException($"Passenger with passport {review.ReviewerPassport} not found.");
            }

            // Create a new Review object
            var newReview = new Review
            {
                ReviewerPassport = review.ReviewerPassport, // Map passport
                FlightNo = review.FlightNo,                 // Map flight number
                Rating = review.Rating,                     // Map rating
                Comment = review.Comment,                   // Map comment (optional field)
                Reviewer = reviewer                         // Set navigation property
            };



            _reviewService.AddReview(newReview);
        }
    }
}
