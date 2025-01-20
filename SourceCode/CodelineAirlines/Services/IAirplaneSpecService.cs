using CodelineAirlines.DTOs.AirplaneSpecDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IAirplaneSpecService
    {
        AirplaneSpecs AddAirplaneSpecs(AirplaneSpecInputDTO airplaneSpecInputDTO);
        List<AirplaneSpecOutputDTO> GetModelsSpecs();
        List<AirplaneSpecs> GetModelsSpecsWithRelatedData();
        AirplaneSpecOutputDTO GetSpecsByModel(string modelName);
        AirplaneSpecs GetSpecsByModelWithRelatedData(string modelName);
    }
}