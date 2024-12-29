using CodelineAirlines.Models;

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
                           .FirstOrDefault(a => a.AirplaneId == id);
        }
    }
}
