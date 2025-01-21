using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IUserService
    {
        void Register(UserInputDTOs userInput);
        public string login(string email, string password);
        public UserOutputDTO GetUserByID(int id);
        public string GenerateJwtToken(string userId, string username,string role);
        public void UpdateUsers(UserInputDTOs userInputDTO, int id);
        public void DeactivateUser(int userId);
        public void ReactivateUser(int userId);

        User GetUserByIdWithRelatedData(int userId);
    }
}