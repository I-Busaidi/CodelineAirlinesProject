using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IPassengerRepository
    {
        void AddPassenger(Passenger passenger);
        bool PassengerExistsForUser(int userId);
    }
}