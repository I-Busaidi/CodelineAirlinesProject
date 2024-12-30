using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

        public string? SeatNo { get; set; }

        [Required]
        public decimal TotalCost { get; set; }
        public string? Meal { get; set; }
        public int Status { get; set; } = 0;
        public DateTime BookingDate { get; set; } = DateTime.Now;

        public Passenger Passenger { get; set; }
        public Flight Flight { get; set; }
        public User User { get; set; }
    }
}
