using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public class SeatTemplateRepository : ISeatTemplateRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor that takes the DbContext
        public SeatTemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adds a SeatTemplate to the database
        public void Add(SeatTemplate seatTemplate)
        {
            _context.SeatTemplates.Add(seatTemplate);  // Add the SeatTemplate to the DbSet

            _context.SaveChanges();
        }
    }
}
