﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodelineAirlines.Models
{
    public class Airplane
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AirplaneId { get; set; }

        [Required]
        public string AirplaneModel { get; set; }

        [Required(ErrorMessage = "Manufacture date is required")]
        public DateOnly ManufactureDate { get; set; }

        [ForeignKey("Airport")]
        public int CurrentAirportId { get; set; }
        public Airport Airport { get; set; }

        public bool IsActive { get; set; } = true;

        [InverseProperty("Airplane")]
        public virtual ICollection<Flight> Flights { get; set; }
    }
}