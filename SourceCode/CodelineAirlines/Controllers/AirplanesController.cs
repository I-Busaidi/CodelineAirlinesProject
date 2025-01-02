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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {


                var airplane = _airplaneService.AddAirplane(airplaneCreateDto);
                return CreatedAtAction(nameof(GetAirplane), new { id = airplane.AirplaneId }, airplane);
            }
            catch (ArgumentNullException ex)
            {
                // Return 400 Bad Request with a specific message
                return BadRequest(new { message = ex.Message });
            }
           
            catch (Exception ex) {

                // Return 500 Internal Server Error for unexpected exceptions
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
          
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

        // Endpoint to update an airplane's details
        [HttpPut("{id}")]
        public IActionResult UpdateAirplane(int id, [FromBody] AirplaneCreateDTO airplaneCreateDto)
        {
            var success = _airplaneService.UpdateAirplane(id, airplaneCreateDto);

            if (!success)
            {
                return NotFound();  // Return 404 if airplane not found
            }

            return NoContent();  // Return 204 No Content if the update was successful
        }

        // Endpoint to delete an airplane
        [HttpDelete("{id}")]
        public IActionResult DeleteAirplane(int id)
        {
            var success = _airplaneService.DeleteAirplane(id);

            if (!success)
            {
                return NotFound();  // Return 404 if airplane not found
            }

            return NoContent();  // Return 204 No Content if the delete was successful
        }
    }
}
