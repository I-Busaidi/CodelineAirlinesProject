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


    }
}
