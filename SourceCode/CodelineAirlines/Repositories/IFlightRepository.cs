using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IFlightRepository
    {
        int AddFlight(Flight flight);
        IEnumerable<Flight> GetAllFlights();
        int UpdateFlight(Flight flight);
        Flight GetFlightById(int id);

        int CancelFlight(Flight flight);
    }
}