using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.UserDTOs
{
    public class UserInputDTOs
    {
        [Required]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email must be in a valid format.")]
        public string UserEmail { get; set; }

        [Required( ErrorMessage = "Email is required.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters long, with at least one letter and one number.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression(@"^(admin|user)$", ErrorMessage = "Role must be either 'admin' or 'user'.")]
        public string UserRole { get; set; }



    }
}
