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
                return null; // Return null or handle not found as per your logic
            }

            var airplaneDto = _mapper.Map<AirplaneOutputDto>(airplane);

            return airplaneDto;
        }
    }
}
