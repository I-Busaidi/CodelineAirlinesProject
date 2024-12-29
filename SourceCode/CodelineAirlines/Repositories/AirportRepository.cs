using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly ApplicationDbContext _context;

        public AirportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string AddAirport(Airport airport)
        {
            _context.Airports.Add(airport);
            _context.SaveChanges();
            return airport.AirportName;
        }
    }
}
