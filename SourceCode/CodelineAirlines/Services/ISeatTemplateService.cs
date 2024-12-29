using CodelineAirlines.DTOs.AirplaneDTOs;

namespace CodelineAirlines.Services
{
    public interface ISeatTemplateService
    {
        void GenerateSeatTemplatesForModel(GenerateSeatTemplateDto dto);
        
    }
}