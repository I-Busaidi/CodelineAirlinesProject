using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IFlightRepository
    {
        int AddFlight(Flight flight);
        IEnumerable<Flight> GetAllFlights();
    }
}