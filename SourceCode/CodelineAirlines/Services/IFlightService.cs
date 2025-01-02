using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IFlightService
    {
        int AddFlight(Flight flightInput);
        List<Flight> GetAllFlightsWithRelatedData();

        List<FlightOutputDTO> GetAllFlights();
        List<Flight> GetFlightsByDateInterval(DateTime startDate, DateTime endDate);
        Flight GetFlightByIdWithRelatedData(int id);
        int UpdateFlightStatus(Flight flight);

        int CancelFlight(Flight flight);
        IEnumerable<Flight> GetAirplaneFlightSchedule(int airplaneId, int flightNo = -1);
    }
}