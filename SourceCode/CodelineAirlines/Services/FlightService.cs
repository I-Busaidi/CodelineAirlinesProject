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

        public int UpdateFlightStatus(Flight flight)
        {
            return _flightRepository.UpdateFlight(flight);
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
