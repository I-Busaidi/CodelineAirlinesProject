using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IPassengerRepository
    {
        void AddPassenger(Passenger passenger);
        bool PassengerExistsForUser(int userId);
        IQueryable<Passenger> GetPassengerByUserId(int userId);
        Passenger GetPassengerByPassport(string passportNumber);
        void UpdatePassenger(Passenger passenger);
        int GetLoyaltyPointsByUserId(int userId);
        Passenger GetByPassport(string passport);
    }
}