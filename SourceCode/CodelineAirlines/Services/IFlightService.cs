using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IFlightService
    {
        int AddFlight(Flight flightInput);
        List<Flight> GetAllFlights();
        List<Flight> GetFlightsByDateInterval(DateTime startDate, DateTime endDate);
        Flight GetPriorFlight(int airplaneId);
        bool IsFlightConflicting(FlightInputDTO flightInput);
        Flight GetFlightByIdWithRelatedData(int id);
        int UpdateFlightStatus(Flight flight);

        int CancelFlight(Flight flight);
    }
}