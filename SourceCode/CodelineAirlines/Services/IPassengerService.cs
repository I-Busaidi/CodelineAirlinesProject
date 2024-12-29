using CodelineAirlines.DTOs.PassengerDTOs;

namespace CodelineAirlines.Services
{
    public interface IPassengerService
    {
        void AddPassenger(PassengerInputDTOs passengerInputDTO, int userId, bool isAdmin);
        PassengerOutputDTO GetPassengerProfile(int userId);
    }
}