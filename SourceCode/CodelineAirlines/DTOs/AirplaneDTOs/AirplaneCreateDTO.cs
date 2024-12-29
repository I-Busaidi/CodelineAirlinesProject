using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.AirplaneDTOs
{
    public class AirplaneCreateDTO
    {
        [Required]
        public string AirplaneModel { get; set; }

        [Required(ErrorMessage = "Manufacture date is required")]
        public DateOnly ManufactureDate { get; set; }

        [Required]
        public int CurrentAirportId { get; set; }
    }
}
