using CodelineAirlines.DTOs.FlightDTOs;

namespace CodelineAirlines.Services
{
    public interface ICompoundService
    {
        int AddFlight(FlightInputDTO flightInput);
    }
}