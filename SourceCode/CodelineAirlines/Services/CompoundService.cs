using ActiveUp.Net.Mail;
using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirplaneSpecDTOs;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.Enums;
using CodelineAirlines.Helpers;
using CodelineAirlines.Helpers.WeatherForecast;
using CodelineAirlines.Models;
using MimeKit.Encodings;
using System.Collections.Immutable;

namespace CodelineAirlines.Services
{
    public class CompoundService : ICompoundService
    {
        private readonly IFlightService _flightService;
        private readonly IAirportService _airportService;
        private readonly IAirplaneService _airplaneService;
        private readonly IBookingService _bookingService;
        private readonly ISeatTemplateService _seatTemplateService;
        private readonly IAirplaneSpecService _airplaneSpecService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IReviewService _reviewService;
        private readonly IPassengerService _passengerService;
        private readonly IAirportLocationService _airportLocationService;
        private readonly WeatherService _weatherService;


        public CompoundService(IFlightService flightService
            , IAirportService airportService
            , IAirplaneService airplaneService
            , IMapper mapper
            , IBookingService bookingService
            , ApplicationDbContext context
            , ISeatTemplateService seatTemplateService
            , IReviewService reviewService
            , IPassengerService passengerService
            , IAirportLocationService airportLocationService
            , WeatherService weatherService
            , IAirplaneSpecService airplaneSpecService)
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
            _airportLocationService = airportLocationService;
            _weatherService = weatherService;
            _airplaneSpecService = airplaneSpecService;
        }

        public (string airportName, string country, string city) AddAirport(AirportControllerInputDTO airportInput)
        {
            var cityDetails = _weatherService.GetWeatherAsync(airportInput.City);
            if (cityDetails == null)
            {
                throw new InvalidOperationException("City name is invalid");
            }
            using (var transcation = _context.Database.BeginTransaction())
            {
                try
                {
                    var airport = _airportService.AddAirport(new AirportInputDTO
                    {
                        AirportName = airportInput.AirportName,
                        Country = cityDetails.Result.sys.country,
                        City = airportInput.City
                    });

                    _airportLocationService.AddAirportLocation(new AirportLocation
                    {
                        Airport = airport,
                        AirportId = airport.AirportId,
                        AirportLatitude = cityDetails.Result.coord.lat,
                        AirportLongitude = cityDetails.Result.coord.lon
                    });

                    _context.SaveChanges();
                    transcation.Commit();

                    return (airport.AirportName, airport.Country, airport.City);
                }
                catch (Exception ex) 
                {
                    transcation.Rollback();
                    throw new InvalidOperationException("An error occured when adding airport" + ex.Message);
                }
            }
        }

        public int ClaculateFlightInputDuration(FlightControllerInput flightControllerInput)
        {
            var airplane = _airplaneService.GetAirplaneByIdWithRelatedData(flightControllerInput.AirplaneId);

            var sourceAirport = _airportService.GetAirportByNameWithRelatedData(flightControllerInput.SourceAirportName);
            var destAirport = _airportService.GetAirportByNameWithRelatedData(flightControllerInput.DestinationAirportName);

            var sourceCoords = _airportLocationService.GetAirportLocation(sourceAirport.AirportId);
            var destCoords = _airportLocationService.GetAirportLocation(destAirport.AirportId);

            double distance = FlightDistanceClaculator.CalculateDistance(sourceCoords.AirportLatitude, 
                sourceCoords.AirportLongitude, 
                destCoords.AirportLatitude, 
                destCoords.AirportLongitude);

            TimeSpan duration = FlightDistanceClaculator.CalculateFlightDuration(distance, airplane.AirplaneSpec.AvgSpeed);

            return AddFlight(new FlightInputDTO
            {
                AirplaneId = flightControllerInput.AirplaneId,
                SourceAirportName = sourceAirport.AirportName,
                DestinationAirportName = destAirport.AirportName,
                Cost = flightControllerInput.Cost,
                Duration = duration,
                ScheduledDepartureDate = flightControllerInput.ScheduledDepartureDate
            });
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

            if (!airplane.IsActive)
            {
                throw new InvalidOperationException("This airplane has been deactivated.");
            }

            var sourceAirport = _airportService.GetAirportByNameWithRelatedData(flightInput.SourceAirportName);
            if (sourceAirport == null)
            {
                throw new KeyNotFoundException("Source airport not found");
            }

            if (!sourceAirport.IsActive)
            {
                throw new InvalidOperationException("Source airport is not currently unavailable.");
            }

            var destinationAirport = _airportService.GetAirportByNameWithRelatedData(flightInput.DestinationAirportName);
            if (destinationAirport == null)
            {
                throw new KeyNotFoundException("Destination airport not found");
            }

            if (!destinationAirport.IsActive)
            {
                throw new InvalidOperationException("Destination airport is not currently unavailable.");
            }

            var flight = _mapper.Map<Flight>(flightInput);
            flight.Airplane = airplane;
            flight.SourceAirport = sourceAirport;
            flight.DestinationAirport = destinationAirport;

            if (!CheckAirplaneAvailability(flight))
            {
                throw new InvalidOperationException("Airplane is not available for this flight, or does not align with the airplane's flight schedule.");
            }

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

            if (newDate == flight.ScheduledDepartureDate)
            {
                throw new InvalidOperationException("New date cannot be the same as original date.");
            }

            flight.ScheduledDepartureDate = newDate;
            flight.ActualDepartureDate = newDate;

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

            if (!airplane.IsActive)
            {
                throw new InvalidOperationException("This airplane has been deactivated.");
            }

            if (!CheckAirplaneAvailability(flight, flightNo))
            {
                throw new InvalidOperationException("Airplane is not available for this flight, or does not align with the airplane's flight schedule.");
            }

            using (var transcation = _context.Database.BeginTransaction())
            {
                try
                {
                    flight.StatusCode = 6;
                    // CancelFlight is a function that only update status not actually cancelling it.
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


            if (flight.ActualDepartureDate != null && flight.ActualDepartureDate > flight.ScheduledDepartureDate)
            {
                departureDate = flight.ActualDepartureDate;
            }
            else
            {
                departureDate = flight.ScheduledDepartureDate;
            }

            if (flight.EstimatedArrivalDate != null && flight.EstimatedArrivalDate > flight.ScheduledArrivalDate)
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
        public (int FlightNo, string? Status) Land(int flightNo)
        {
            var flight = _flightService.GetFlightByIdWithRelatedData(flightNo);
            if (flight == null)
            {
                throw new KeyNotFoundException("Could retrieve flight information.");
            }

            if (flight.StatusCode == 1 || flight.StatusCode == 2)
            {
                throw new InvalidOperationException("Flight is still in boarding state.");
            }
            else if (flight.StatusCode == 4)
            {
                throw new InvalidOperationException("Flight has already arrived.");
            }
            else if (flight.StatusCode == 5)
            {
                throw new InvalidOperationException("Flight has been cancelled.");
            }
            else if (flight.StatusCode == 0 || flight.StatusCode == 6)
            {
                throw new InvalidOperationException("Flight is still pending.");
            }

            flight.StatusCode = 4;

            UpdateAirplaneCurrentAirportId(flight.AirplaneId, flight.DestinationAirport.AirportName);

            return (_flightService.CancelFlight(flight), Enum.GetName(typeof(FlightStatus), flight.StatusCode));
        }

        private void UpdateAirplaneCurrentAirportId(int airplaneId, string airportName)
        {
            var airplane = _airplaneService.GetAirplaneByIdWithRelatedData(airplaneId);
            if (airplane == null)
            {
                throw new KeyNotFoundException("Could not find flight airplane.");
            }

            var airport = _airportService.GetAirportByNameWithRelatedData(airportName);
            if (airport == null)
            {
                throw new KeyNotFoundException("Could not find landing airport.");
            }

            airplane.CurrentAirportId = airport.AirportId;

            _airplaneService.UpdateAirplane(airplane);
        }

        private bool CheckAirplaneAvailability(Flight flight, int flightNo = -1)
        {
            var airplaneFlights = _flightService.GetAirplaneFlightSchedule(flight.AirplaneId, flightNo);
            if (airplaneFlights != null)
            {
                var flightsList = airplaneFlights.Append(flight).OrderBy(f => f.ScheduledDepartureDate).ToList();

                for (int i = 0;i < flightsList.Count; i++)
                {
                    if (i == 0)
                    {
                        if (flightsList[0].SourceAirportId != flightsList[0].Airplane.CurrentAirportId)
                        {
                            return false;
                        }
                    }

                    if ((i+1) <= flightsList.Count - 1)
                    {
                        if (flightsList[i].DestinationAirportId != flightsList[i+1].SourceAirportId)
                        {
                            return false;
                        }

                        if (flightsList[i].ScheduledArrivalDate.AddHours(1) > flightsList[i + 1].ScheduledDepartureDate)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
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

        public List<SeatsOutputDTO> GetAvailableSeats(int flightNo)
        {
            var flight = _flightService.GetFlightByIdWithRelatedData(flightNo);
            var seats = _seatTemplateService.GetSeatTemplatesByModel(flight.Airplane.AirplaneModel);
            if (seats == null || seats.Count() == 0)
            {
                throw new KeyNotFoundException("Could not find seat template.");
            }

            var availableSeats = seats.Where(s => !flight.Bookings.Any(b => b.SeatNo == s.SeatNumber)).ToList();
            List<SeatsOutputDTO> availableSeatsList = _mapper.Map<List<SeatsOutputDTO>>(availableSeats);
            return availableSeatsList;
        }

        public string AddAirplaneModel(AirplaneSpecInputDTO airplaneSpecInput)
        {
            using (var transcation = _context.Database.BeginTransaction())
            {
                try
                {
                    var airplaneSpecs = _airplaneSpecService.AddAirplaneSpecs(airplaneSpecInput);

                    _seatTemplateService.GenerateSeatTemplatesForModel(airplaneSpecInput.SeatTemplate);

                    _context.SaveChanges();
                    transcation.Commit();

                    return airplaneSpecInput.Model ;
                }
                catch (Exception ex)
                {
                    transcation.Rollback();
                    throw new InvalidOperationException("An error occured when adding model " + ex.Message);
                }
            }
        }
    }
}
