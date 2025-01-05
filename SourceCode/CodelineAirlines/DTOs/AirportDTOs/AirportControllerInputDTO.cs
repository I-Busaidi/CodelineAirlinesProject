using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.AirportDTOs
{
    public class AirportControllerInputDTO
    {
        [Required(ErrorMessage = "Airport name is required")]
        [StringLength(50, ErrorMessage = "Airport name cannot exceed 50 characters")]
        public string AirportName { get; set; }

        [Required(ErrorMessage = "City name is required")]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        public string City { get; set; }
    }
}
