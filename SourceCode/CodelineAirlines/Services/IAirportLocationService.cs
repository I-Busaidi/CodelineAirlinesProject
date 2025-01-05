using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IAirportLocationService
    {
        AirportLocationOutputDTO GetAirportLocation(int airportId);
        void AddAirportLocation(AirportLocation airportLocation);
        void DeleteAirportLocation(AirportLocation airportLocation);
        void UpdateAirportLocation(AirportLocation airportLocation);
    }
}