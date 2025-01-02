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
        Flight GetPriorFlight(int airplaneId);
        bool IsFlightConflicting(FlightInputDTO flightInput);
        bool IsFlightConflictingForReschedule(Flight flightInput);
        Flight GetFlightByIdWithRelatedData(int id);
        int UpdateFlightStatus(Flight flight);

        int CancelFlight(Flight flight);
        Flight GetPriorFlightForReschedule(int airplaneId, int flightNo);
    }
}