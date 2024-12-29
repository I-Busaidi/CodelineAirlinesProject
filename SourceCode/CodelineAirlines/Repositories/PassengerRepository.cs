using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly ApplicationDbContext _context;

        public PassengerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddPassenger(Passenger passenger)
        {
            _context.Passengers.Add(passenger);
            _context.SaveChanges();
        }
        public bool PassengerExistsForUser(int userId)
        {
            return _context.Passengers.Any(p => p.UserId == userId);
        }

    }
}
