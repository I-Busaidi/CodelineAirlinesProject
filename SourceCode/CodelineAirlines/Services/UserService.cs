using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace CodelineAirlines.Services
{
    public class UserService
    {
        private readonly IUserRepository _userrepo;

        public UserService(IUserRepository userrepo)
        {
            _userrepo = userrepo;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create()) //uses SHA-256 to hash the password securely.
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert byte to hexadecimal
                }
                return builder.ToString();
            }
        }
        public void Register(UserInputDTOs userInput)
        {

            var user = new User
            {
                UserName = userInput.Name,
                UserEmail = userInput.Email,
                Password = HashPassword(userInput.Password),  // Hash the password before storing it
         
                UserRole = userInput.Role
            };
            _userrepo.AddUser(user);


        }
    }
}
