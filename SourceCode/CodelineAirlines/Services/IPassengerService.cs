using CodelineAirlines.DTOs.PassengerDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IPassengerService
    {
        void AddPassenger(PassengerInputDTOs passengerInputDTO, int userId, bool isAdmin);
        PassengerOutputDTO GetPassengerProfile(int userId);
        void UpdatePassengerDetails(int userId, PassengerInputDTOs passengerInputDTO);
        int GetLoyaltyPoints(int userId);
        Passenger GetPassengerByPassport(string passport);

    }
}