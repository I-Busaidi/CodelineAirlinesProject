using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Repositories
{
    public class AirplaneRepository : IAirplaneRepository
    {
        private readonly ApplicationDbContext _context;

        public AirplaneRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddAirplane(Airplane airplane)
        {
            _context.Airplanes.Add(airplane);

            _context.SaveChanges();
        }

        public Airplane GetById(int id)
        {
            return _context.Airplanes
                .Include(a => a.Airport)
                .Include(a => a.AirplaneSpec)
                .FirstOrDefault(a => a.AirplaneId == id);
        }

        public List<Airplane> GetAll()
        {
            return _context.Airplanes // Retrieve all airplanes synchronously
             .Include(a => a.Airport)
             .Include(a => a.AirplaneSpec)
             .ToList();
        }

        // This method updates an existing airplane's details.
        public void Update(Airplane airplane)
        {
            _context.Airplanes.Update(airplane);  // Updates the entity in the context

            _context.SaveChanges();
        }

        // Delete an existing airplane
        public void Delete(Airplane airplane)
        {
            _context.Airplanes.Remove(airplane);  // Remove the airplane from the context

            _context.SaveChanges();
        }
    }
}
