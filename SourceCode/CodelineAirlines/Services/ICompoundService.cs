﻿using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.Enums;

namespace CodelineAirlines.Services
{
    public interface ICompoundService
    {
        int AddFlight(FlightInputDTO flightInput);
        (int, string) UpdateFlightStatus(int flightId, FlightStatus flightStatus);

        (int flightNo, int BookingsCount) CancelFlight(int flightId, string condition);
    }
}