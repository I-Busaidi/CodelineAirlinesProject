using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class AirportController : ControllerBase
    {
        private readonly IAirportService _airportService;
        private readonly ICompoundService _compoundService;

        public AirportController(IAirportService airportService, ICompoundService compoundService)
        {
            _airportService = airportService;
            _compoundService = compoundService;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AddAirport")]
        public IActionResult AddAirport([FromBody] AirportControllerInputDTO airportInputDTO)
        {
            try
            {
                var airport = _compoundService.AddAirport(airportInputDTO);
                return Created(string.Empty, $"{airport.airportName} has been added. City: {airport.city}, Country: {airport.country}");
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAirports")]
        public IActionResult GetAirports(
            int pageNumber = 1, 
            int pageSize = 10, 
            string country = "", 
            string city = "", 
            string airportName = "")
        {
            try
            {
                var airports = _airportService.GetAllAirports()
                    .Where(ap => ap.Country.ToLower().Trim().Contains(country.ToLower().Trim())
                    & ap.City.ToLower().Trim().Contains(city.ToLower().Trim())
                    & ap.AirportName.ToLower().Trim().Contains(airportName.ToLower().Trim()))
                    .Skip((pageNumber - 1)* pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(airports);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }

        [HttpGet("GetAirportByName/{name}")]
        public IActionResult GetAirportByName(string name)
        {
            try
            {
                var airport = _airportService.GetAirportByName(name);
                return Ok(airport);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateAirportInfo/{airportId}")]
        public IActionResult UpdateAirport(int airportId, [FromBody] AirportInputDTO airportInput)
        {
            try
            {
                int updatedAirportId = _airportService.UpdateAirport(airportInput, airportId);
                return Ok(updatedAirportId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeactivateAirport/{airportId}")]
        public IActionResult DeactivateAirport(int airportId)
        {
            try
            {
                int deactivatedAirportId = _airportService.DeactivateAirport(airportId);
                return Ok(deactivatedAirportId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("ReactivateAirport/{airportId}")]
        public IActionResult ReactivateAirport(int airportId)
        {
            try
            {
                int reactivatedAirportId = _airportService.ReactivateAirport(airportId);
                return Ok(reactivatedAirportId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteAirport/{airportId}")]
        public IActionResult DeleteAirport(int airportId)
        {
            try
            {
                _airportService.DeleteAirport(airportId);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }
    }
}
