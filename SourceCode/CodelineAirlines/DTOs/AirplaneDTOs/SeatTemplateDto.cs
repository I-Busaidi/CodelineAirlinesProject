using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.AirplaneDTOs
{
    public class GenerateSeatTemplateDto
    {
        [Required]
        public string AirplaneModel { get; set; }

        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
        public int NumberOfAisles { get; set; } = 1;

        public int[] EconomySeatsPerRow { get; set; } = new int[3] { 3, 0, 3 };
        public int[] BusinessSeatsPerRow { get; set; } = new int[3] { 2, 0, 2 };
        public int[] FirstClassSeatsPerRow { get; set; } = new int[3] { 1, 0, 1};
    }
}
