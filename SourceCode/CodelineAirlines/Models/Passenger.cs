using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodelineAirlines.Models
{
    public class Passenger
    {
        [Key]
        [Required(ErrorMessage = "Passport number is required")]
        [StringLength(30)]
        public string Passport { get; set; }

        [Required]
        [StringLength(50)]
        public string PassengerName { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Passenger date of birth is required")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Nationality is required")]
        [StringLength(20)]
        public string Nationality { get; set; }

        public int LoyaltyPoints { get; set; } = 0;

        [InverseProperty("Passenger")]
        public virtual ICollection<Booking> Bookings { get; set; }

        [InverseProperty("Reviewer")]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
