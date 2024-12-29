using AutoMapper;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace CodelineAirlines.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userrepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userrepo, IMapper mapper)
        {
            _userrepo = userrepo;
            _mapper = mapper;
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

            User NewUser = _mapper.Map<User>(userInput);
            _userrepo.AddUser(NewUser);


        }




    }
}
