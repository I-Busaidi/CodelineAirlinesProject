﻿using System.ComponentModel.DataAnnotations;

namespace CodelineAirlines.DTOs.AirportDTOs
{
    public class AirportOutputDTO
    {
        public string AirportName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }
    }
}