﻿using AutoMapper;
using CodelineAirlines.DTOs.FlightDTOs;
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
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CompoundService(IFlightService flightService, IAirportService airportService, IAirplaneService airplaneService, IMapper mapper, IBookingService bookingService, ApplicationDbContext context)
        {
            _context = context;
            _bookingService = bookingService;
            _flightService = flightService;
            _airportService = airportService;
            _airplaneService = airplaneService;
            _mapper = mapper;
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

        private bool CheckAirplaneAvailability(Airplane airplane, Airport srcAirport, FlightInputDTO flightInput)
        {
            var priorFlight = _flightService.GetPriorFlight(airplane.AirplaneId);
            if (priorFlight != null)
            {
                if (priorFlight.DestinationAirportId != srcAirport.AirportId)
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
    }
}
