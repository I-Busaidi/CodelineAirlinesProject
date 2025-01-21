using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.PassengerDTOs
{
    public class PassengerOutputDTO
    {
        public string PassengerName { get; set; }
        public string Passport { get; set; }
        public string Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Nationality { get; set; }
        public int LoyaltyPoints { get; set; }
    }
}
