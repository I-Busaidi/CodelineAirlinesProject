using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace CodelineAirlines.Models
{
    [PrimaryKey(nameof(AirportId), nameof(AirportLatitude), nameof(AirportLongitude))]
    public class AirportLocation
    {
        [Required]
        [ForeignKey("Airport")]
        public int AirportId { get; set; }

        [Required]
        public double AirportLatitude { get; set; }

        [Required]
        public double AirportLongitude { get; set; }

        public virtual Airport Airport { get; set; }
    }
}
