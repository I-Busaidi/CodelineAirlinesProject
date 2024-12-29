using CodelineAirlines.DTOs.UserDTOs;

namespace CodelineAirlines.Services
{
    public interface IUserService
    {
        void Register(UserInputDTOs userInput);
        public string login(string email, string password);

    }
}