using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface ISeatTemplateService
    {
        void GenerateSeatTemplatesForModel(GenerateSeatTemplateDto dto);

        // Retrieves seat templates by airplane model name, ordered by SeatCost in descending order
        IEnumerable<SeatTemplate> GetSeatTemplatesByModel(string airplaneModel);
    }
}