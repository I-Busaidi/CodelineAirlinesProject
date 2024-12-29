using CodelineAirlines.DTOs.AirportDTOs;

namespace CodelineAirlines.Services
{
    public interface IAirportService
    {
        string AddAirport(AirportInputDTO airportInputDTO);
    }
}