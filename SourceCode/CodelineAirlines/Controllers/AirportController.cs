using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AirportController : ControllerBase
    {
        private readonly IAirportService _airportService;

        public AirportController(IAirportService airportService)
        {
            _airportService = airportService;
        }

        [HttpPost("AddAirport")]
        public IActionResult AddAirport([FromBody] AirportInputDTO airportInputDTO)
        {
            try
            {
                string addedAirport = _airportService.AddAirport(airportInputDTO);
                return Created(string.Empty, addedAirport);
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
    }
}
