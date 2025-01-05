using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IAirportService
    {
        Airport AddAirport(AirportInputDTO airportInputDTO);
        List<AirportOutputDTO> GetAllAirports();
        AirportOutputDTO GetAirportByName(string name);
        int UpdateAirport(AirportInputDTO airportInput, int id);
        void DeleteAirport(int id);
        int DeactivateAirport(int id);
        int ReactivateAirport(int id);
        Airport GetAirportByNameWithRelatedData(string name);
    }
}