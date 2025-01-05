using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodelineAirlines.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        [ForeignKey("Passenger")]
        public string PassengerPassport { get; set; }

        [Required]
        [ForeignKey("Flight")]
        public int FlightNo { get; set; }

        [Required]
        public string Class {  get; set; }

        public string? SeatNo { get; set; }

        public int LoyaltyPointsUsed { get; set; }

        [Required]
        public decimal TotalCost { get; set; }
        public string? Meal { get; set; }
        public int Status { get; set; } = 0;
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Passenger Passenger { get; set; }
        [JsonIgnore]
        public Flight Flight { get; set; }
    }
}
