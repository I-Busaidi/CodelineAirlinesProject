using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly ApplicationDbContext _context;

        public FlightRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public int AddFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            _context.SaveChanges();

            return flight.FlightNo;
        }

        public IEnumerable<Flight> GetAllFlights()
        {
            return _context.Flights
                .Include(f => f.Airplane)
                .Include(f => f.SourceAirport)
                .Include(f => f.DestinationAirport)
                .Include(f => f.Bookings);
        }
    }
}
