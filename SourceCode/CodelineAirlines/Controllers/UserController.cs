using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Helpers;
using CodelineAirlines.Models;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public UserController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserInputDTOs userInputDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                _userService.Register(userInputDTO);
                string subject = "Welcome!";
                string body = $"Hello, {userInputDTO.UserName} " +
                            $"Thank you for registering in AirLine Codeline.";//Mesg Email 
                await _emailService.SendEmailAsync(userInputDTO.UserEmail, subject, body);   // Send email
                return Ok(new { Message = "User added successfully", userInputDTO.UserName});
             
        

            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Message = "An error occurred while adding the user", Error = errorMessage });

            }


        }
        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login(string email, string password)
        {
            try
            {
                string token = _userService.login(email, password);

                if (token == null)
                {

                    return Unauthorized(new { Message = "Invalid Credentials" });
                }

                return Ok(new { token });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetUserDetails")]
        public IActionResult GetUserDetails()
        {
            try
            {
                string token = JwtHelper.ExtractToken(Request);
                int userId = int.Parse(JwtHelper.GetClaimValue(token, JwtRegisteredClaimNames.Sub));
                var user = _userService.GetUserByID(userId);
                if (user == null)
                {
                    return NotFound("user not found");
                }
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle the exception if the user is not found in the service layer
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions and return 500 Internal Server Error
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }

        }
        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser([FromBody] UserInputDTOs userInputDTO, int id)
        {
            try
            {
                _userService.UpdateUsers(userInputDTO, id);
                return Ok(new { Message = "User updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the user.", Error = ex.Message });
            }
        }
        [HttpDelete("DeactivateUser/{id}")]
     
        public IActionResult DeactivateUser(int id)
        {
            try
            {
                _userService.DeactivateUser(id);
                return Ok(new { Message = "User deactivated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deactivating the user.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]//Allow only admin 
        [HttpPost("ReactivateUser/{id}")]
        public IActionResult ReactivateUser(int id)
        {
            try
            {
                _userService.ReactivateUser(id);
                return Ok(new { Message = "User reactivated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while reactivating the user.", Error = ex.Message });
            }
        }



    }
}
