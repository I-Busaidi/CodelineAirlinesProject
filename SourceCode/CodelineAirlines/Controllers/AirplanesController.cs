using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirplanesController : ControllerBase
    {
        private readonly IAirplaneService _airplaneService;

        public AirplanesController(IAirplaneService airplaneService)
        {
            _airplaneService = airplaneService;
        }

        [HttpPost]
        public IActionResult AddAirplane([FromBody] AirplaneCreateDTO airplaneCreateDto)
        {
            if (ModelState.IsValid)
            {
                var airplane = _airplaneService.AddAirplane(airplaneCreateDto);
                return CreatedAtAction(nameof(GetAirplane), new { id = airplane.AirplaneId }, airplane);
            }

            return BadRequest(ModelState);
        }

        // Sample method to get an airplane (for the "CreatedAtAction" response)
        [HttpGet("{id}")]
        public IActionResult GetAirplane(int id)
        {
            var airplaneDto = _airplaneService.GetById(id);

            if (airplaneDto == null)
            {
                return NotFound(); // Return 404 if airplane is not found
            }

            return Ok(airplaneDto); // Return the AirplaneOutputDto as JSON response
        }

        // Endpoint to get all airplanes
        [HttpGet]
        public IActionResult GetAllAirplanes()
        {
            var airplaneDtos = _airplaneService.GetAll();  // Get all airplanes via service

            if (airplaneDtos == null || !airplaneDtos.Any())
            {
                return NotFound();  // Return 404 if no airplanes are found
            }

            return Ok(airplaneDtos);  // Return the list of airplane DTOs as a JSON response
        }
    }
}
