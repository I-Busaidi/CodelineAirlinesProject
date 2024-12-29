using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IAirplaneRepository
    {
        void AddAirplane(Airplane airplane); // Method to add a new Airplane
        Airplane GetById(int id); // Method to get an airplane by ID
        List<Airplane> GetAll();  // Method to get all airplanes
        void Update(Airplane airplane);  // Method to update an airplane
        void Delete(Airplane airplane);  // Method to delete an airplane
    }
}