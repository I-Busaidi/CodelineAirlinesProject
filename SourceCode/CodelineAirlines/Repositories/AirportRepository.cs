using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly ApplicationDbContext _context;

        public AirportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Airport AddAirport(Airport airport)
        {
            _context.Airports.Add(airport);
            return airport;
        }

        public IEnumerable<Airport> GetAllAirports()
        {
            return _context.Airports;
        }

        public Airport GetAirportById(int id)
        {
            return _context.Airports
                .Include(ap => ap.Airplanes)
                .Include(ap => ap.DestinationFlights)
                .Include(ap => ap.SourceFlights)
                .FirstOrDefault(ap => ap.AirportId == id);
        }

        public Airport GetAirportByName(string name)
        {
            return _context.Airports.FirstOrDefault(ap => ap.AirportName == name);
        }

        public int UpdateAirport(Airport airport)
        {
            _context.Airports.Update(airport);

            return airport.AirportId;
        }

        public void DeleteAirport(Airport airport)
        {
            _context.Airports.Remove(airport);
        }
    }
}
