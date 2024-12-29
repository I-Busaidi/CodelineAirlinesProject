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
                           .FirstOrDefault(a => a.AirplaneId == id);
        }

        public List<Airplane> GetAll()
        {
            return _context.Airplanes // Retrieve all airplanes synchronously
             .Include(a => a.Airport)
             .ToList();
        }
    }
}
