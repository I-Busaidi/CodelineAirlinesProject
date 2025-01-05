using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public class AirportLocationRepository : IAirportLocationRepository
    {
        private readonly ApplicationDbContext _context;

        public AirportLocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public AirportLocation GetAirportLocation(int id)
        {
            return _context.AirportLocations.FirstOrDefault(a => a.AirportId == id);
        }

        public void AddLocation(AirportLocation airportLocation)
        {
            _context.AirportLocations.Add(airportLocation);
        }

        public void UpdateLocation(AirportLocation airportLocation)
        {
            _context.AirportLocations.Update(airportLocation);
        }

        public void DeleteLocation(AirportLocation airportLocation)
        {
            _context.AirportLocations.Remove(airportLocation);
        }
    }
}
