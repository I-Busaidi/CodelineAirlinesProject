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

        public IEnumerable<SeatTemplate> GetAllModels()
        {
            return _context.SeatTemplates;
        }

        // Adds a SeatTemplate to the database
        public void Add(SeatTemplate seatTemplate)
        {
            _context.SeatTemplates.Add(seatTemplate);  // Add the SeatTemplate to the DbSet
        }

        // Retrieves seat templates by airplane model name, ordered by SeatCost in descending order
        public IEnumerable<SeatTemplate> GetSeatTemplatesByModel(string airplaneModel)
        {
            return _context.SeatTemplates
                .Where(st => st.AirplaneModel == airplaneModel)  // Filter by AirplaneModel
                .OrderByDescending(st => st.SeatCost)  // Order by SeatCost in descending order
                .ToList();  // Execute the query and return the result as a list
        }

        // Deletes all SeatTemplates associated with a given Airplane Model
        public void DeleteByModel(string airplaneModel)
        {
            var seatTemplates = _context.SeatTemplates
                .Where(st => st.AirplaneModel == airplaneModel)  // Find all seat templates for the model
                .ToList();

            if (seatTemplates.Any())
            {
                _context.SeatTemplates.RemoveRange(seatTemplates);  // Remove all matching seat templates

                _context.SaveChanges();
            }
        }
    }
}
