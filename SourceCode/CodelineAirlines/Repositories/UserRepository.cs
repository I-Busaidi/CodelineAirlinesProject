using CodelineAirlines.Models;

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
                throw new Exception(ex.InnerException.Message);
            }
        }
        public User GetUserForLogin(string email, string password)
        {
            return _context.Users.Where(u => u.UserName == email & u.Password == password).FirstOrDefault();

        }
        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(a => a.UserId == id);
        }

        public void UpdateUser(User user, int id)
        {
            {

                var currenruser = GetById(id);
                if (currenruser != null)
                {
                    currenruser.UserName = user.UserName;
                    currenruser.UserEmail = user.UserEmail;
                    currenruser.Password = user.Password;


                    _context.Users.Update(currenruser);
                    _context.SaveChanges();
                }
            }
        }
        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

    }
}
