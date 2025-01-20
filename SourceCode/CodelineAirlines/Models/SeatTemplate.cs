using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodelineAirlines.Models
{
    [PrimaryKey(nameof(AirplaneModel), nameof(SeatNumber))]
    public class SeatTemplate
    {
        [ForeignKey("AirplaneSpec")]
        [Required(ErrorMessage = "Airplane model is required")]
        public string AirplaneModel { get; set; }
        public AirplaneSpecs AirplaneSpec { get; set; }

        [Required(ErrorMessage = "Seat type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Seat number is required")]
        public string SeatNumber { get; set; }

        public int SeatLocation { get; set; } = 0;

        public decimal SeatCost { get; set; } = 0;
    }
}
