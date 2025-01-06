using CodelineAirlines.DTOs.PassengerDTOs;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class PassengerController : ControllerBase
    {
        private readonly IPassengerService _passengerService;
        private readonly IUserService _userService;
        public PassengerController(IPassengerService passengerService)
        {
            _passengerService = passengerService;
        }

        [HttpPost("RegisterAsPassenger")]
        public IActionResult AddPassenger([FromQuery] PassengerInputDTOs passengerInputDTO)
        {
            try
            {
                // Retrieve the current user's ID from the token (assuming JWT is used)
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var isAdmin = User.IsInRole("admin");

                // Call the service method to add the passenger
                _passengerService.AddPassenger(passengerInputDTO, userId, isAdmin);

                return Ok(new { Message = "Passenger profile created successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the passenger profile.", Error = ex.Message });
            }
        }
  
       
            [HttpGet("Profile")]
            public IActionResult GetPassengerProfile()
            {
                try
                {
                    // Get the user ID from the JWT token (assuming JWT is used)
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Fetch the passenger profile based on userId
                var passengerProfile = _passengerService.GetPassengerProfile(userId);

                    return Ok(passengerProfile);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
            }

        [HttpPut("UpdatePassengerDetails")]
        public IActionResult UpdatePassengerProfile([FromBody] PassengerInputDTOs passengerInputDTO)
        {
            try
            {
                // Get the user ID from the JWT token
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Update the passenger details through the service
                _passengerService.UpdatePassengerDetails(userId, passengerInputDTO);

                return Ok(new { Message = "Passenger details updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("GetLoyaltyPoints")]
        public IActionResult GetLoyaltyPoints()
        {
            try
            {
                // Get the user ID from the JWT token
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Get loyalty points from service
                var loyaltyPoints = _passengerService.GetLoyaltyPoints(userId);

                // Return the loyalty points as part of the response
                return Ok(new { LoyaltyPoints = loyaltyPoints });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
