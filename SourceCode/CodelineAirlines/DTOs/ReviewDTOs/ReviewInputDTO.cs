using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodelineAirlines.DTOs.ReviewDTOs
{
    public class ReviewInputDTO
    {    //ignored during input,use for output only
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] // Available in output but ignored for input

        public int? ReviewId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Reviewer Passport number cannot exceed 50 characters.")]
        public string ReviewerPassport { get; set; }
        [Required]
        public int FlightNo { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Rating must be between 0 and 10.")]
        public decimal Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }
    }
}
