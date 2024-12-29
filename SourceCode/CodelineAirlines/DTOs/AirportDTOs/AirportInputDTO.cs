using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.AirportDTOs
{
    public class AirportInputDTO
    {
        [Required(ErrorMessage = "Airport name is required")]
        [StringLength(50, ErrorMessage = "Airport name cannot exceed 50 characters")]
        public string AirportName { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [StringLength(50, ErrorMessage = "Country name cannot exceed 50 characters")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City name is required")]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        public string City { get; set; }
    }
}
