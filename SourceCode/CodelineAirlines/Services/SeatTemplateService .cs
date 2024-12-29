using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;

namespace CodelineAirlines.Services
{
    public class SeatTemplateService : ISeatTemplateService
    {
        private readonly ISeatTemplateRepository _seatTemplateRepository;
        private readonly IMapper _mapper;

        public SeatTemplateService(ISeatTemplateRepository seatTemplateRepository, IMapper mapper)
        {
            _seatTemplateRepository = seatTemplateRepository;
            _mapper = mapper;
        }

        // This method will automatically generate seat templates for the given airplane model and seat distribution
        public void GenerateSeatTemplatesForModel(GenerateSeatTemplateDto dto)
        {
            var seatTemplates = new List<SeatTemplate>();

            

            

            // Generate Seat Templates for First Class
            seatTemplates.AddRange(GenerateSeatsForClass(dto.AirplaneModel, "First Class", dto.FirstClassSeats, 500));  // 500 for first class seat cost

            // Generate Seat Templates for Business Class
            seatTemplates.AddRange(GenerateSeatsForClass(dto.AirplaneModel, "Business", dto.BusinessSeats, 200, seatTemplates.Count +1));  // 200 for business seat cost

            // Generate Seat Templates for Economy Class
            seatTemplates.AddRange(GenerateSeatsForClass(dto.AirplaneModel, "Economy", dto.EconomySeats, 100, seatTemplates.Count +1));  // 100 for economy seat cost

            // Add to database
            foreach (var seatTemplate in seatTemplates)
            {
                _seatTemplateRepository.Add(seatTemplate);
            }
        }

        // Helper method to generate seats for a specific class
        private List<SeatTemplate> GenerateSeatsForClass(string airplaneModel, string seatType, int totalSeats, decimal seatCost, int startingNumber = 1)
        {
            var seatTemplates = new List<SeatTemplate>();
            int rows = (totalSeats / 10) + startingNumber -1;  // Assuming there are 10 seats per row (3 columns on each side of the aisle)

            // Generate the seat templates
            int seatCount = 1;
            for (int row = startingNumber; row <= rows; row++)
            {
                for (int column = 1; column <= 10; column++)  // Assuming 10 seats per row
                {
                    string seatNumber = $"{row}{(char)('A' + column - 1)}";  // Seat numbers: 1A, 1B, etc.
                    bool isWindowSeat = (column == 1 || column == 10);  // Window seats are the first and last in each row

                    var seatTemplate = new SeatTemplate
                    {
                        AirplaneModel = airplaneModel,
                        Type = seatType,
                        SeatNumber = seatNumber,
                        IsWindowSeat = isWindowSeat,
                        SeatCost = seatCost
                    };

                    seatTemplates.Add(seatTemplate);
                    seatCount++;
                }
            }

            return seatTemplates;
        }

        // Retrieves seat templates by airplane model name, ordered by SeatCost in descending order
        public IEnumerable<SeatTemplate> GetSeatTemplatesByModel(string airplaneModel)
        {
            return _seatTemplateRepository.GetSeatTemplatesByModel(airplaneModel);
        }
    }
}
