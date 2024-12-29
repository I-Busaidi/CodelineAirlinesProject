using CodelineAirlines.DTOs.AirportDTOs;

namespace CodelineAirlines.Services
{
    public interface IAirportService
    {
        string AddAirport(AirportInputDTO airportInputDTO);
        List<AirportOutputDTO> GetAllAirports();
        AirportOutputDTO GetAirportByName(string name);
        int UpdateAirport(AirportInputDTO airportInput, int id);
        void DeleteAirport(int id);
        int DeactivateAirport(int id);
    }
}