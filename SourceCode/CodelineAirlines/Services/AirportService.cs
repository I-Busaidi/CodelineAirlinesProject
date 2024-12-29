using AutoMapper;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CodelineAirlines.Services
{
    public class AirportService : IAirportService
    {
        private readonly IAirportRepository _airportRepository;
        private readonly IMapper _mapper;

        public AirportService(IAirportRepository airportRepository, IMapper mapper)
        {
            _airportRepository = airportRepository;
            _mapper = mapper;
        }

        public string AddAirport(AirportInputDTO airportInputDTO)
        {
            if (airportInputDTO == null)
            {
                throw new ArgumentNullException("Input is null");
            }

            if (string.IsNullOrWhiteSpace(airportInputDTO.AirportName))
            {
                throw new InvalidOperationException("Airport name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(airportInputDTO.Country))
            {
                throw new InvalidOperationException("Airport country cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(airportInputDTO.City))
            {
                throw new InvalidOperationException("Airport city cannot be empty");
            }

            Airport newAirport = _mapper.Map<Airport>(airportInputDTO);
            return _airportRepository.AddAirport(newAirport);
        }

        public List<AirportOutputDTO> GetAllAirports()
        {
            var airports = _airportRepository.GetAllAirports().ToList();
            if (airports == null || airports.Count == 0)
            {
                throw new InvalidOperationException("No airports available");
            }

            var airportsOutput = _mapper.Map<List<AirportOutputDTO>>(airports);
            return airportsOutput;
        }

        public AirportOutputDTO GetAirportByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Invalid name");
            }

            var airport = _airportRepository.GetAirportByName(name);
            if (airport == null)
            {
                throw new KeyNotFoundException("Could not find airport");
            }

            return _mapper.Map<AirportOutputDTO>(airport);
        }
    }
}
