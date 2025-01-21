using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirplaneSpecDTOs;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SeatTemplatesController : ControllerBase
    {
        private readonly ISeatTemplateService _seatTemplateService;
        private readonly ICompoundService _compoundService;

        public SeatTemplatesController(ISeatTemplateService seatTemplateService, ICompoundService compoundService)
        {
            _seatTemplateService = seatTemplateService;
            _compoundService = compoundService;
        }

        // Endpoint to generate seat templates for an airplane model
        [Authorize(Roles = "admin")]
        [HttpPost("generate")]
        public IActionResult GenerateSeatTemplates([FromBody] GenerateSeatTemplateDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.AirplaneModel))
                {
                    return BadRequest("Invalid data.");
                }

                // Call the service layer to generate seat templates
                _seatTemplateService.GenerateSeatTemplatesForModel(dto);

                return Ok("Seat templates successfully generated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to get seat templates by airplane model, ordered by SeatCost in descending order
        [Authorize(Roles = "admin")]
        [HttpGet("by-model/{airplaneModel}")]
        public IActionResult GetSeatTemplatesByModelOrderedByCost(string airplaneModel)
        {
            var seatTemplates = _seatTemplateService.GetSeatTemplatesByModel(airplaneModel);

            // Check if any seat templates were found
            if (seatTemplates == null || !seatTemplates.Any())
            {
                return NotFound($"No seat templates found for airplane model '{airplaneModel}'.");
            }

            // Return the seat templates ordered by cost
            return Ok(seatTemplates);
        }

        // Endpoint to delete seat templates by airplane model
        [Authorize(Roles = "admin")]
        [HttpDelete("delete/{airplaneModel}")]
        public IActionResult DeleteSeatTemplatesByModel(string airplaneModel)
        {
            // Call the service to delete the seat templates by model
            _seatTemplateService.DeleteSeatTemplatesByModel(airplaneModel);

            // Return a success response
            return NoContent(); // HTTP 204 No Content to indicate successful deletion
        }

        [AllowAnonymous]
        [HttpGet("ViewAvailableModels")]
        public IActionResult ViewAvailableModels()
        {
            try
            {
                var models = _seatTemplateService.GetAllAvailableModels();
                return Ok(models);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("AddAirplaneSpec")]
        public IActionResult AddAirplaneSpec([FromBody] AirplaneSpecInputDTO airplaneSpecInput)
        {
            try
            {
                var result = _compoundService.AddAirplaneModel(airplaneSpecInput);
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
    }
}
