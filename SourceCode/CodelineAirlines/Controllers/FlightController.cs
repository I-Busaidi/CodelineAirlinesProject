using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.Enums;
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

        //[Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
        [HttpPatch("UpdateFlightStatus/{flightNo}")]
        public IActionResult UpdateFlightStatus(int flightNo, [FromBody] FlightStatusRequest statusRequest)
        {
            try
            {
                var result = _compoundService.UpdateFlightStatus(flightNo, statusRequest.FlightStatus);
                return Ok("Status of flight with number " + result.Item1 + " has been updated to " + result.Item2);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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

        [Authorize(Roles = "admin")]
        [HttpDelete("CancelFlight/{FlightNo}")]
        public IActionResult CancelFlight(int FlightNo, string condition)
        {
            try
            {
                var result = _compoundService.CancelFlight(FlightNo, condition);
                return Ok($"Flight canceled.\nFlight Number: {result.flightNo}\nNumber of bookings canceled: {result.BookingsCount}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
