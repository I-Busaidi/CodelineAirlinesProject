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
        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login(string email, string password)
        {
            string token = _userService.login(email, password);

            if (token == null)
            {
            
                return Unauthorized(new { Message = "Invalid Credentials" });
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
