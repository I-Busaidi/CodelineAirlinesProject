using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IAirplaneService
    {
        Airplane AddAirplane(AirplaneCreateDTO airplaneCreateDto);
        bool DeactivateAirplane(int id);
        bool DeleteAirplane(int id);
        Airplane GetAirplaneByIdWithRelatedData(int id);
        List<AirplaneOutputDto> GetAll();
        AirplaneOutputDto GetById(int id);
        bool ReactivateAirplane(int id);
        void UpdateAirplane(Airplane airplane);
        bool UpdateAirplane(int id, AirplaneCreateDTO airplaneCreateDto);
    }
}