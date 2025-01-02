using AutoMapper;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.Enums;
using CodelineAirlines.Migrations;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;

namespace CodelineAirlines.Services
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;
        private readonly IMapper _mapper;

        public FlightService(IFlightRepository flightRepository, IMapper mapper)
        {
            _flightRepository = flightRepository;
            _mapper = mapper;
        }

        public int AddFlight(Flight flightInput)
        {
            if (flightInput == null)
            {
                throw new ArgumentNullException("Flight details not found");
            }
            flightInput.ActualDepartureDate = flightInput.ScheduledDepartureDate;
            return _flightRepository.AddFlight(flightInput);
        }

        public List<Flight> GetAllFlightsWithRelatedData()
        {
            var flights = _flightRepository.GetAllFlights()
                .OrderBy(f => f.StatusCode)
                .ToList();

            return flights;
        }

        public List<FlightOutputDTO> GetAllFlights()
        {
            var flights = _flightRepository.GetAllFlights()
                .OrderBy(f => f.StatusCode)
                .ToList();

            if (flights == null || flights.Count == 0)
            {
                throw new InvalidOperationException("No flights found");
            }

            return _mapper.Map<List<FlightOutputDTO>>(flights);
        }

        public List<Flight> GetFlightsByDateInterval(DateTime startDate, DateTime endDate)
        {
            var flights = _flightRepository.GetAllFlights()
                .Where(f => f.ScheduledDepartureDate >=  startDate && f.ScheduledArrivalDate <= endDate)
                .ToList();

            return flights;
        }

        public IEnumerable<Flight> GetAirplaneFlightSchedule(int airplaneId, int flightNo = -1)
        {
            return _flightRepository.GetAllFlights()
                .Where(f => f.AirplaneId == airplaneId 
                && (f.StatusCode < 4 || f.StatusCode == 6) 
                && f.FlightNo != flightNo);
        }

        public (int FlightNo, string? Status) StartAirplaneBoarding(int flightNo)
        {
            var flight = _flightRepository.GetFlightById(flightNo);
            if (flight == null)
            {
                throw new KeyNotFoundException("Could retrieve flight information.");
            }

            if (flight.StatusCode == 1 || flight.StatusCode == 2)
            {
                throw new InvalidOperationException("Flight is already in boarding state.");
            }
            else if (flight.StatusCode == 3)
            {
                throw new InvalidOperationException("Flight has already taken off.");
            }
            else if (flight.StatusCode == 4)
            {
                throw new InvalidOperationException("Flight has already arrived to destination.");
            }
            else if (flight.StatusCode == 5)
            {
                throw new InvalidOperationException("Flight has been cancelled.");
            }

            if (flight.Airplane.CurrentAirportId != flight.SourceAirportId)
            {
                throw new InvalidOperationException("Airplane is not available in the airport yet.");
            }

            if (flight.ScheduledDepartureDate.AddMinutes(15) >= DateTime.Now)
            {
                flight.StatusCode = 1;
            }
            else
            {
                flight.StatusCode = 2;
            }

            flight.ActualDepartureDate = DateTime.Now;

            return (_flightRepository.UpdateFlight(flight), Enum.GetName(typeof(FlightStatus), flight.StatusCode));
        }

        public (int FlightNo, string? Status) StartFlight(int flightNo)
        {
            var flight = _flightRepository.GetFlightById(flightNo);
            if (flight == null)
            {
                throw new KeyNotFoundException("Could retrieve flight information.");
            }

            if (flight.StatusCode == 0 || flight.StatusCode == 6)
            {
                throw new InvalidOperationException("Flight is not in boarding state.");
            }
            else if (flight.StatusCode == 3)
            {
                throw new InvalidOperationException("Flight has already taken off.");
            }
            else if (flight.StatusCode == 4)
            {
                throw new InvalidOperationException("Flight has already arrived to destination.");
            }
            else if (flight.StatusCode == 5)
            {
                throw new InvalidOperationException("Flight has been cancelled.");
            }

            flight.StatusCode = 3;

            flight.ActualDepartureDate = DateTime.Now;

            return (_flightRepository.UpdateFlight(flight), Enum.GetName(typeof(FlightStatus), flight.StatusCode));
        }

        public int CancelFlight(Flight flight)
        {
            return _flightRepository.CancelFlight(flight);
        }

        public Flight GetFlightByIdWithRelatedData(int id)
        {
            var flight = _flightRepository.GetFlightById(id);
            if (flight == null)
            {
                throw new KeyNotFoundException("Flight could not be found");
            }

            return flight;
        }
    }
}
