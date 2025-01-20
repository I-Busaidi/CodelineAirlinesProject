using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Repositories
{
    public class AirplaneSpecRepository : IAirplaneSpecRepository
    {
        private readonly ApplicationDbContext _context;

        public AirplaneSpecRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<AirplaneSpecs> GetAirplaneModelsSpecs()
        {
            return _context.AirplaneSpecs
                .Include(a => a.Seats)
                .Include(a => a.Airplane);
        }

        public AirplaneSpecs GetAirplaneSpecsByModel(string model)
        {
            return _context.AirplaneSpecs
                .Include(a => a.Seats)
                .Include(a => a.Airplane)
                .FirstOrDefault(a => a.Model == model);
        }

        public AirplaneSpecs AddAirplaneSpecs(AirplaneSpecs airplaneSpecs)
        {
            _context.AirplaneSpecs.Add(airplaneSpecs);
            return airplaneSpecs;
        }

        public string UpdateAirplaneSpec(AirplaneSpecs airplaneSpecs)
        {
            _context.AirplaneSpecs.Update(airplaneSpecs);
            _context.SaveChanges();
            return airplaneSpecs.Model;
        }
    }
}
