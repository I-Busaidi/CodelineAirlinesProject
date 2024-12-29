using AutoMapper;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CodelineAirlines.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userrepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userrepo, IMapper mapper, IConfiguration configuration)
        {
            _userrepo = userrepo;
            _mapper = mapper;
            _configuration = configuration;
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

            if (string.IsNullOrEmpty(userInput.Password))
            {
                throw new Exception("Password is null or empty");
            }
            User NewUser = _mapper.Map<User>(userInput);
            if (string.IsNullOrEmpty(NewUser.UserEmail))
            {
                throw new Exception("UserEmail is null or empty");
            }
            // Hash the password
            NewUser.Password = HashPassword(userInput.Password);
            // Add the user to the repository
            _userrepo.AddUser(NewUser);

        }
        public string GenerateJwtToken(string userId, string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.UniqueName, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                     };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string login(string email, string password)
        {
            
            // Hash the entered password
            string HashedPassword = HashPassword(password);
            var user = _userrepo.GetUserForLogin(email, HashedPassword);
            if (user == null)
            {
                return null;
            }

            else
            {
                return GenerateJwtToken(user.UserId.ToString(), user.UserName);
            }
        }

        public UserOutputDTO GetUserByID(int id)
        {
            try
            {
                var user = _userrepo.GetById(id);
                var outPutUser = new UserOutputDTO
                {
               
                    Name = user.UserName,
                    Email=user.UserEmail,
                    Role = user.UserRole
                

                };

                return outPutUser;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message);
            }

        }



    }
}
