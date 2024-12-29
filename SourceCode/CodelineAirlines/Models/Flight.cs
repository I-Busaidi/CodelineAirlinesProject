using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.Models
{
    public class Flight
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FlightNo { get; set; }

        [Required(ErrorMessage = "Source airport ID is required")]
        [ForeignKey("SourceAirport")]
        public int SourceAirportId { get; set; }

        [Required(ErrorMessage = "Destination airport ID is required")]
        [ForeignKey("DestinationAirport")]
        public int DestinationAirportId { get; set; }

        [Required(ErrorMessage = "Airplane ID is required")]
        [ForeignKey("Airplane")]
        public int AirplaneId { get; set; }

        [Required]
        [Range(0.01, int.MaxValue, ErrorMessage = "Flight cost must be greater than 0")]
        public decimal Cost { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public DateTime ScheduledDepartureDate { get; set; }

        [Required]
        public DateTime? ActualDepartureDate { get; set; }

        public DateTime ScheduledArrivalDate => ScheduledDepartureDate.Add(Duration);

        public DateTime? EstimatedArrivalDate => ActualDepartureDate?.Add(Duration);

        [Required]
        [Range(0, 15, ErrorMessage = "Status codes are from 0 to 15")]
        public int StatusCode { get; set; } = 0; // 

        [Range(0, 10, ErrorMessage = "Flight Rating must be between 0 to 10")]
        public decimal? FlightRating { get; set; }

        // Navigation Properties
        public Airport SourceAirport { get; set; }
        public Airport DestinationAirport { get; set; }
        public Airplane Airplane { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }

        [InverseProperty("FlightReview")]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
