using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatTemplatesController : ControllerBase
    {
        private readonly ISeatTemplateService _seatTemplateService;

        public SeatTemplatesController(ISeatTemplateService seatTemplateService)
        {
            _seatTemplateService = seatTemplateService;
        }

        // Endpoint to generate seat templates for an airplane model
        [HttpPost("generate")]
        public IActionResult GenerateSeatTemplates([FromBody] GenerateSeatTemplateDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.AirplaneModel))
            {
                return BadRequest("Invalid data.");
            }

            // Call the service layer to generate seat templates
            _seatTemplateService.GenerateSeatTemplatesForModel(dto);

            return Ok("Seat templates successfully generated.");
        }

        // Endpoint to get seat templates by airplane model, ordered by SeatCost in descending order
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
    }
}
