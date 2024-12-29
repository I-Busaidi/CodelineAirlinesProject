using CodelineAirlines.DTOs.UserDTOs;

namespace CodelineAirlines.Services
{
    public interface IUserService
    {
        void Register(UserInputDTOs userInput);
        public string login(string email, string password);
        public UserOutputDTO GetUserByID(int id);
        public string GenerateJwtToken(string userId, string username);
        public void UpdateUsers(UserInputDTOs userInputDTO, int id);
    }
}