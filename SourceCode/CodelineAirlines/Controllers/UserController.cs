using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddUser([FromBody] UserInputDTOs userInputDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                _userService.Register(userInputDTO);
                return Ok(new { Message = "User added successfully", userInputDTO.UserName});

            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Message = "An error occurred while adding the user", Error = errorMessage });

            }


        }
        [HttpGet("Login")]
        public IActionResult Login(string email, string password)
        {
            string token = _userService.login(email, password);

            if (token == null)
            {
                return BadRequest(new { Message = "Invalid Credintials" });
            }

            return Ok(new { token });
        }
        [Authorize]
        [HttpGet("GetUserDetails")]
        public IActionResult GetUserDetails(int id)
        {
            var user = _userService.GetUserByID(id);
            if (user == null)
            {
                return NotFound("user not found");
            }
            return Ok(user);

        }


    }
}
