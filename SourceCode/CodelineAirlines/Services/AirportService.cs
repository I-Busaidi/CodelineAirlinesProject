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

        public Airport AddAirport(AirportInputDTO airportInputDTO)
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
        public Airport GetAirportByNameWithRelatedData(string name)
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

            return airport;
        }

        public int UpdateAirport (AirportInputDTO airportInput, int id)
        {
            var airport = _airportRepository.GetAirportById(id);
            if (airport == null)
            {
                throw new KeyNotFoundException("Could not find airport");
            }

            if (string.IsNullOrWhiteSpace(airportInput.AirportName))
            {
                throw new InvalidOperationException("Airport name cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(airportInput.Country))
            {
                throw new InvalidOperationException("Airport country cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(airportInput.City))
            {
                throw new InvalidOperationException("Airport city cannot be empty");
            }

            var airportNameCheck = _airportRepository.GetAirportByName(airportInput.AirportName);

            if (airportNameCheck != null && airportNameCheck.AirportId != airport.AirportId)
            {
                throw new InvalidOperationException("Airport name is already used by another airport");
            }

            airport.AirportName = airportInput.AirportName;
            airport.Country = airportInput.Country;
            airport.City = airportInput.City;

            return _airportRepository.UpdateAirport(airport);
        }

        public int DeactivateAirport(int id)
        {
            var airport = _airportRepository.GetAirportById(id);
            if (airport == null)
            {
                throw new KeyNotFoundException("Airport could not be found");
            }

            if (airport.Airplanes.Any(p => p.IsActive))
            {
                throw new InvalidOperationException("Airport has active airplanes");
            }

            if (airport.DestinationFlights.Any(df => df.StatusCode != 10) || airport.SourceFlights.Any(df => df.StatusCode != 10))
            {
                throw new InvalidOperationException("Airport has pending flights");
            }

            airport.IsActive = false;
            return _airportRepository.UpdateAirport(airport);
        }

        public int ReactivateAirport(int id)
        {
            var airport = _airportRepository.GetAirportById(id);
            if (airport == null)
            {
                throw new KeyNotFoundException("Airport could not be found");
            }

            if (airport.IsActive)
            {
                throw new InvalidOperationException("Airport is already active");
            }

            airport.IsActive = true;
            return _airportRepository.UpdateAirport(airport);
        }

        public void DeleteAirport(int id)
        {
            var airport = _airportRepository.GetAirportById(id);
            if (airport == null)
            {
                throw new KeyNotFoundException("Airport could not be found");
            }

            if (airport.Airplanes.Any(p => p.IsActive))
            {
                throw new InvalidOperationException("Airport has active airplanes");
            }

            if (airport.DestinationFlights.Any(df => df.StatusCode != 10) || airport.SourceFlights.Any(df => df.StatusCode != 10))
            {
                throw new InvalidOperationException("Airport has pending flights");
            }

            _airportRepository.DeleteAirport(airport);
        }
    }
}
