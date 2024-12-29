using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IPassengerRepository
    {
        void AddPassenger(Passenger passenger);
        bool PassengerExistsForUser(int userId);
        Passenger GetPassengerByUserId(int userId);
        void UpdatePassenger(Passenger passenger);
        int GetLoyaltyPointsByUserId(int userId);
    }
}