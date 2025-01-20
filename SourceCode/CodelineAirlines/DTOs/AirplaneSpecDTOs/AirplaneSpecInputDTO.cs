using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;
using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.AirplaneSpecDTOs
{
    public class AirplaneSpecInputDTO
    {
        [Required]
        public string Model { get; set; }

        public double AvgSpeed { get; set; } = 850;

        [Required]
        public double LuggageCapacity { get; set; }

        public GenerateSeatTemplateDto SeatTemplate { get; set; }
    }
}
