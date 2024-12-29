using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IAirplaneService
    {
        Airplane AddAirplane(AirplaneCreateDTO airplaneCreateDto);
        AirplaneOutputDto GetById(int id);
        List<AirplaneOutputDto> GetAll();
        bool UpdateAirplane(int id, AirplaneCreateDTO airplaneCreateDto);  // Update method
        bool DeleteAirplane(int id);  // Method to delete an airplane
    }
}