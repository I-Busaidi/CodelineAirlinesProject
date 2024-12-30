using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.ReviewDTOs
{
    public class ReviewInputDTO
    {
        [Required]
        public int FlightNo { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
        public decimal Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }
    }
}
