using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodelineAirlines.Models
{
    public class Airport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AirportId { get; set; }

        [Required(ErrorMessage = "Airport name is required")]
        [StringLength(50, ErrorMessage = "Airport name cannot exceed 50 characters")]
        public string AirportName { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [StringLength(50, ErrorMessage = "Country name cannot exceed 50 characters")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City name is required")]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        public string City { get; set; }

        public bool IsActive { get; set; } = true;

        [InverseProperty("Airport")]
        public virtual ICollection<Airplane> Airplanes { get; set; }

        [InverseProperty("SourceAirport")]
        public virtual ICollection<Flight> SourceFlights { get; set; }

        [InverseProperty("DestinationAirport")]
        public virtual ICollection<Flight> DestinationFlights { get; set; }
    }
}
