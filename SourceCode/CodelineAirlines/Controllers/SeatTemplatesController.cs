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
    }
}
