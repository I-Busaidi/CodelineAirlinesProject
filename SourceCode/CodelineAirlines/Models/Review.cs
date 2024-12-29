using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        [Required]
        [ForeignKey("Reviewer")]
        public string ReviewerPassport { get; set; }

        [Required]
        [ForeignKey("FlightReview")]
        public int FlightNo { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 to 10")]
        public decimal Rating { get; set; }
        public string? Comment { get; set; }

        // Navigation Properties
        public Passenger Reviewer { get; set; }
        public Flight FlightReview { get; set; }
    }
}
