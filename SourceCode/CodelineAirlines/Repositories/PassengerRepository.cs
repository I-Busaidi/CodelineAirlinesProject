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

        public Passenger GetPassengerByUserId(int userId)
        {
            // Find the passenger with userId
            var passenger = _context.Passengers
                .FirstOrDefault(p => p.UserId == userId);

            return passenger;
        }

        public Passenger GetPassengerByPassport(string passportNumber)
        {
            // Find the passenger by their passport number
            return _context.Passengers.FirstOrDefault(p => p.Passport == passportNumber);
        }

        public void UpdatePassenger(Passenger passenger)
        {
            // Find the passenger to update
            var existingPassenger = _context.Passengers
                .FirstOrDefault(p => p.UserId == passenger.UserId);

            if (existingPassenger != null)
            {
                // Update the passenger properties
                existingPassenger.Passport = passenger.Passport;
                existingPassenger.Gender = passenger.Gender;
                existingPassenger.BirthDate = passenger.BirthDate;
                existingPassenger.Nationality = passenger.Nationality;
                existingPassenger.LoyaltyPoints = passenger.LoyaltyPoints;

                // Update the passenger in the context
                _context.Passengers.Update(existingPassenger);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Passenger profile not found.");
            }
        }
        public Passenger GetByPassport(string passport)
        {
            return _context.Set<Passenger>().FirstOrDefault(p => p.Passport == passport);
        }
        public int GetLoyaltyPointsByUserId(int userId)
        {
            // Find the passenger by UserId
            var passenger = _context.Passengers.FirstOrDefault(p => p.UserId == userId);

            // If no passenger profile is found, throw an exception
            if (passenger == null)
            {
                throw new InvalidOperationException("Passenger profile not found.");
            }

            return passenger.LoyaltyPoints;
        }
    }
}
