using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IAirplaneService
    {
        Airplane AddAirplane(AirplaneCreateDTO airplaneCreateDto);
        AirplaneOutputDto GetById(int id);
    }
}