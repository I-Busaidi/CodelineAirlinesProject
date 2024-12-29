using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;

namespace CodelineAirlines.Services
{
    public class AirplaneService : IAirplaneService
    {
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly IMapper _mapper;

        public AirplaneService(IAirplaneRepository airplaneRepository, IMapper mapper)
        {
            _airplaneRepository = airplaneRepository;
            _mapper = mapper;
        }

        public Airplane AddAirplane(AirplaneCreateDTO airplaneCreateDto)
        {
            var airplane = _mapper.Map<Airplane>(airplaneCreateDto);

            // Add airplane to repository and save changes
            _airplaneRepository.AddAirplane(airplane);
            return airplane;
        }

        public AirplaneOutputDto GetById(int id)
        {
            // Retrieve the airplane entity by ID
            var airplane = _airplaneRepository.GetById(id);

            if (airplane == null)
            {
                return null;
            }

            var airplaneDto = _mapper.Map<AirplaneOutputDto>(airplane);

            return airplaneDto;
        }

        // Retrieve all airplanes and map them to AirplaneOutputDto
        public List<AirplaneOutputDto> GetAll()
        {
            var airplanes = _airplaneRepository.GetAll();  // Fetch airplanes from the repository

            // Use AutoMapper to map the list of Airplanes to AirplaneOutputDto
            return _mapper.Map<List<AirplaneOutputDto>>(airplanes);
        }

        // Update an airplane's details
        public bool UpdateAirplane(int id, AirplaneCreateDTO airplaneCreateDto)
        {
            var airplane = _airplaneRepository.GetById(id);

            if (airplane == null)
            {
                return false;  // Airplane not found
            }

            // Map the incoming AirplaneCreateDto to the existing Airplane entity
            _mapper.Map(airplaneCreateDto, airplane);

            // Update the airplane in the repository
            _airplaneRepository.Update(airplane);

            return true;  // Successfully updated
        }
    }
}
