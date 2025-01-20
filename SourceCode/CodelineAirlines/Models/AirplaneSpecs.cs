using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodelineAirlines.Models
{
    public class AirplaneSpecs
    {
        [Key]
        public string Model { get; set; }

        public double AvgSpeed { get; set; } = 850;

        public int PassengerCapacity { get; set; }

        [Required]
        public double LuggageCapacity { get; set; }

        public virtual ICollection<Airplane> Airplane { get; set; }
        public virtual ICollection<SeatTemplate> Seats { get; set; }
    }
}
