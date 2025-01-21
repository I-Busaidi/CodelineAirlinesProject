using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public int AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return user.UserId; // Return the newly created user's ID
            }

            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public User GetUserForLogin(string email, string password)
        {
            return _context.Users.Where(u => u.UserEmail == email && u.Password == password).FirstOrDefault();

        }
        public User GetById(int id)
        {
            return _context.Users.Include(u => u.Passengers).ThenInclude(p => p.Bookings).FirstOrDefault(u => u.UserId == id);
        }

        public void UpdateUser(User user)
        {

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void DeactivateUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            user.isActive = false; // Deactivate the user
            _context.SaveChanges();
        }

        public void ReactivateUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            user.isActive = true; // Reactivate the user
            _context.SaveChanges();
        }


    }
}
