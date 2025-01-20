using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IAirplaneSpecRepository
    {
        AirplaneSpecs AddAirplaneSpecs(AirplaneSpecs airplaneSpecs);
        IQueryable<AirplaneSpecs> GetAirplaneModelsSpecs();
        AirplaneSpecs GetAirplaneSpecsByModel(string model);
        string UpdateAirplaneSpec(AirplaneSpecs airplaneSpecs);
    }
}