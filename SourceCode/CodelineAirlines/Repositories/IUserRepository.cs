using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IUserRepository
    {
        int AddUser(User user);
        void Delete(int id);
        User GetById(int id);
        User GetUserForLogin(string email, string password);
        void UpdateUser(User user);
        public void DeactivateUser(int userId);
    }
}