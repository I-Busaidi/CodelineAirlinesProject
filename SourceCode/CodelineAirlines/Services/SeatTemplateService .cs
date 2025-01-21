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

        public List<string> GetAllAvailableModels()
        {
            var models = _seatTemplateRepository.GetAllModels().DistinctBy(m => m.AirplaneModel).Select(m => m.AirplaneModel).ToList();

            return models;
        }

        // This method will automatically generate seat templates for the given airplane model and seat distribution
        public void GenerateSeatTemplatesForModel(GenerateSeatTemplateDto dto)
        {
            var seatTemplates = new List<SeatTemplate>();

            if (dto.FirstClassSeats != 0 && dto.FirstClassSeats % dto.FirstClassSeatsPerRow.Sum() != 0)
            {
                throw new InvalidOperationException($"First class number of seats must be multiples of seats per row entered ({dto.FirstClassSeatsPerRow})");
            }

            if (dto.BusinessSeats != 0 && dto.BusinessSeats % dto.BusinessSeatsPerRow.Sum() != 0)
            {
                throw new InvalidOperationException($"Business class number of seats must be multiples of seats per row entered ({dto.BusinessSeatsPerRow})");
            }

            if (dto.EconomySeats % dto.EconomySeatsPerRow.Sum() != 0)
            {
                throw new InvalidOperationException($"Economy class number of seats must be multiples of seats per row entered ({dto.EconomySeatsPerRow})");
            }

            if (dto.EconomySeats < 50)
            {
                throw new InvalidOperationException("Economy class number of seats must be at least 50");
            }

            int seatsCount = 1;

            // Generate Seat Templates for First Class
            if (dto.FirstClassSeats != 0)
            {
                seatTemplates.AddRange(GenerateSeatsForClass(dto.AirplaneModel, "First Class", dto.FirstClassSeats, 0.15m, dto.FirstClassSeatsPerRow, seatsCount, dto.NumberOfAisles));  // +15% for first class seat cost
                seatsCount = seatTemplates.Count + 1;
            }

            // Generate Seat Templates for Business Class
            if (dto.BusinessSeats != 0)
            {
                seatTemplates.AddRange(GenerateSeatsForClass(dto.AirplaneModel, "Business", dto.BusinessSeats, 0.1m, dto.BusinessSeatsPerRow, seatsCount, dto.NumberOfAisles));  // +10% for business seat cost
                seatsCount = seatTemplates.Count + 1;
            }

            // Generate Seat Templates for Economy Class
            seatTemplates.AddRange(GenerateSeatsForClass(dto.AirplaneModel, "Economy", dto.EconomySeats, 0.05m, dto.EconomySeatsPerRow, seatsCount, dto.NumberOfAisles));  // +5% for economy seat cost

            // Add to database
            foreach (var seatTemplate in seatTemplates)
            {
                _seatTemplateRepository.Add(seatTemplate);
            }
        }

        // Helper method to generate seats for a specific class
        private List<SeatTemplate> GenerateSeatsForClass(
            string airplaneModel,
            string seatType,
            int totalSeats,
            decimal seatCost,
            int[] seatsPerRow,
            int startingNumber = 1,
            int aisles = 1)
        {
            var seatTemplates = new List<SeatTemplate>();
            int rows = (totalSeats + seatsPerRow.Sum() - 1) / seatsPerRow.Sum() + startingNumber - 1; // Calculate total rows needed.

            // Generate the seat templates
            for (int row = startingNumber; row <= rows; row++)
            {
                for (int column = 1; column <= seatsPerRow.Sum(); column++)
                {
                    string seatNumber = $"{row}{(char)('A' + column - 1)}"; // Seat numbers: 1A, 1B, etc.

                    // Determine seat location type
                    bool isWindowSeat = (column == 1 || column == seatsPerRow.Sum()); // First and last column in the row.
                    bool isAisleSeat = aisles == 1
                        ? (column == seatsPerRow[0] || column == seatsPerRow[0] + 1) // For single-aisle planes.
                        : (column == seatsPerRow[0] || column == seatsPerRow[0] + 1 || column == seatsPerRow[0]+seatsPerRow[1] || column == seatsPerRow[0] + seatsPerRow[1] + 1); // For double-aisle planes.
                    bool isMiddleSeat = !isWindowSeat && !isAisleSeat; // Remaining seats are middle seats.

                    int location = isWindowSeat ? 0 : isAisleSeat ? 1 : 2; // Map to custom location codes: 0 = Window, 1 = Aisle, 2 = Middle.

                    var seatTemplate = new SeatTemplate
                    {
                        AirplaneModel = airplaneModel,
                        Type = seatType,
                        SeatNumber = seatNumber,
                        SeatLocation = location,
                        SeatCost = seatCost
                    };

                    seatTemplates.Add(seatTemplate);
                }
            }

            return seatTemplates;
        }

        // Retrieves seat templates by airplane model name, ordered by SeatCost in descending order
        public IEnumerable<SeatTemplate> GetSeatTemplatesByModel(string airplaneModel)
        {
            return _seatTemplateRepository.GetSeatTemplatesByModel(airplaneModel);
        }

        // Deletes all SeatTemplates by Airplane Model
        public void DeleteSeatTemplatesByModel(string airplaneModel)
        {
            _seatTemplateRepository.DeleteByModel(airplaneModel);
        }
    }
}
