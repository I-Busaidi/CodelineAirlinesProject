using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly ICompoundService _compoundService;

        public FlightController(IFlightService flightService, ICompoundService compoundService)
        {
            _flightService = flightService;
            _compoundService = compoundService;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AddFlight")]
        public IActionResult AddFlight([FromBody] FlightInputDTO flightInput)
        {
            try
            {
                int newFlightId = _compoundService.AddFlight(flightInput);
                return Ok(newFlightId);
            }
            catch (InvalidOperationException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
