using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodelineAirlines.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [RegularExpression(@"^[A-Za-z'-]+(?: [A-Za-z'-]+)*$", ErrorMessage = "Name must have letters only")]
        [StringLength(50, ErrorMessage = "User name must not exceed 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "User email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(50, ErrorMessage = "Email must not exceed 50 characters")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(15)]
        public string UserRole { get; set; }

        public bool isActive { get; set; } = true;

        public virtual ICollection<Passenger> Passengers { get; set; }
    }
}
