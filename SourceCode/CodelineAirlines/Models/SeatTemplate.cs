using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.Models
{
    [PrimaryKey(nameof(AirplaneModel), nameof(SeatNumber))]
    public class SeatTemplate
    {
        [Required(ErrorMessage = "Airplane model is required")]
        public string AirplaneModel { get; set; }

        [Required(ErrorMessage = "Seat type is required")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Seat number is required")]
        public string SeatNumber { get; set; }

        public bool IsWindowSeat { get; set; } = false;

        public decimal SeatCost { get; set; } = 0;
    }
}
