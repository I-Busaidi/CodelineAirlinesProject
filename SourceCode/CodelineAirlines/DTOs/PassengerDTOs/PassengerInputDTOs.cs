using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.PassengerDTOs
{
    public class PassengerInputDTOs
    {
        [Required(ErrorMessage = "Passport number is required")]
        [StringLength(30)]
        public string Passport { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Passenger date of birth is required")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Nationality is required")]
        [StringLength(20)]
        public string Nationality { get; set; }

    }
}
