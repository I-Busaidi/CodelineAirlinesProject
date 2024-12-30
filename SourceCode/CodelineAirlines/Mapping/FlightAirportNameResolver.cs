using AutoMapper;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Services;

namespace CodelineAirlines.Mapping
{
    public class SourceAirportNameResolver : IValueResolver<FlightInputDTO, Flight, int>
    {
        private readonly IAirportService _airportService;

        public SourceAirportNameResolver(IAirportService airportService)
        {
            _airportService = airportService;
        }

        public int Resolve(FlightInputDTO source, Flight destination, int member, ResolutionContext context)
        {
            var airport = _airportService.GetAirportByNameWithRelatedData(source.SourceAirportName);
            return airport?.AirportId ?? -1;
        }
    }

    public class DestinationAirportNameResolver : IValueResolver<FlightInputDTO, Flight, int>
    {
        private readonly IAirportService _airportService;

        public DestinationAirportNameResolver(IAirportService airportService)
        {
            _airportService = airportService;
        }

        public int Resolve(FlightInputDTO source, Flight destination, int member, ResolutionContext context)
        {
            var airport = _airportService.GetAirportByNameWithRelatedData(source.DestinationAirportName);
            return airport?.AirportId ?? -1;
        }
    }
}
