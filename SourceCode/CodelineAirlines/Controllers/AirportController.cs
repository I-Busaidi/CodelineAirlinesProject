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
    }
}
