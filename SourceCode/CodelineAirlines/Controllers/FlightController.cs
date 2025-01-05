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

        [Authorize(Roles = "admin")]
        [HttpPost("AddFlight")]
        public IActionResult AddFlight([FromBody] FlightControllerInput flightInput)
        {
            try
            {
                int newFlightId = _compoundService.ClaculateFlightInputDuration(flightInput);
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
        [HttpPatch("UpdateFlightStatusToBoarding/{flightNo}")]
        public IActionResult UpdateFlightStatusToBoarding(int flightNo)
        {
            try
            {
                var result = _flightService.StartAirplaneBoarding(flightNo);
                return Ok("Status of flight with number " + result.Item1 + " has been updated to Boarding(" + result.Item2+")");
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
        [HttpPatch("UpdateFlightStatusToInFlight/{flightNo}")]
        public IActionResult UpdateFlightStatusToInFlight(int flightNo)
        {
            try
            {
                var result = _flightService.StartFlight(flightNo);
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
        [HttpPatch("UpdateFlightStatusToArrived/{flightNo}")]
        public IActionResult UpdateFlightStatusToArrived(int flightNo)
        {
            try
            {
                var result = _compoundService.Land(flightNo);
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

        [AllowAnonymous]
        [HttpGet("GetFlightDetails/{flightNo}")]
        public IActionResult GetFlightDetails(int flightNo)
        {
            try
            {
                var result = _compoundService.GetFlightDetails(flightNo);
                return Ok(result);
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

        [HttpPut("RescheduleFlight/{flightNo}/{newDate}")]
        public IActionResult RescheduleFlight(int flightNo, DateTime newDate, int airplaneId = -1)
        {
            try
            {
                var result = _compoundService.RescheduleFlight(flightNo, newDate, airplaneId);
                return Ok($"Flight {result.FlightNo} has been rescheduled to {result.NewDepartureDate.ToString("dd/MM/yyyy ddd ~ hh:mm:ss tt")}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetFlights")]
        public IActionResult GetFlights
            (
                TimeSpan? minDuration = null,
                TimeSpan? maxDuration = null,
                DateTime? fromDate = null,
                DateTime? toDate = null,
                int pageSize = 10,
                int pageNumber = 1,
                string? sourceAirport = "",
                string? destAirport = "",
                string? status = "",
                string? airplaneModel = "",
                decimal minCost = 0,
                decimal maxCost = int.MaxValue
            )
        {
            if ( minDuration == null )
            {
                minDuration = TimeSpan.Zero;
            }
            if ( maxDuration == null )
            {
                maxDuration = TimeSpan.MaxValue;
            }
            if ( fromDate == null )
            {
                fromDate = DateTime.MinValue;
            }
            if ( toDate == null )
            {
                toDate = DateTime.MaxValue;
            }

            try
            {
                var result = _flightService.GetAllFlights()
                    .Where(f => NormalizeString(f.Status).Contains(NormalizeString(status))
                    & NormalizeString(f.AirplaneModel).Contains(NormalizeString(airplaneModel))
                    & NormalizeString(f.SourceAirportName).Contains(NormalizeString(sourceAirport))
                    & NormalizeString(f.DestinationAirportName).Contains(NormalizeString(destAirport))
                    & f.ScheduledDepartureDate >= fromDate
                    & f.ScheduledDepartureDate <= toDate
                    & f.Cost >= minCost
                    & f.Cost <= maxCost
                    & f.Duration >= minDuration
                    & f.Duration <= maxDuration)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(result);
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

        private string NormalizeString(string input)
        {
            return string.Concat(input.Where(c => !char.IsWhiteSpace(c))).ToLower();
        }
    }
}
