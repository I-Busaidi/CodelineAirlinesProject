using CodelineAirlines.DTOs.UserDTOs;

namespace CodelineAirlines.Services
{
    public interface IUserService
    {
        void Register(UserInputDTOs userInput);
    }
}