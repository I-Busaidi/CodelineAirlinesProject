using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IAirportLocationRepository
    {
        AirportLocation GetAirportLocation(int id);
        void AddLocation(AirportLocation airportLocation);
        void DeleteLocation(AirportLocation airportLocation);
        void UpdateLocation(AirportLocation airportLocation);
    }
}