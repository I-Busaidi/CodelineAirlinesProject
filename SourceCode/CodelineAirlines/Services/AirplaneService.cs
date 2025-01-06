using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;

namespace CodelineAirlines.Services
{
    public class AirplaneService : IAirplaneService
    {
        private readonly IAirplaneRepository _airplaneRepository;
        private readonly ISeatTemplateService _seatTemplateService;
        private readonly IMapper _mapper;

        public AirplaneService(IAirplaneRepository airplaneRepository, IMapper mapper, ISeatTemplateService seatTemplateService)
        {
            _airplaneRepository = airplaneRepository;
            _mapper = mapper;
            _seatTemplateService = seatTemplateService;
        }

        public Airplane AddAirplane(AirplaneCreateDTO airplaneCreateDto)
        {
            var seatTemplate = _seatTemplateService.GetSeatTemplatesByModel(airplaneCreateDto.AirplaneModel);
            if (seatTemplate == null || seatTemplate.Count() == 0)
            {
                throw new InvalidOperationException("This model does not exist in the templates.");
            }
            var airplane = _mapper.Map<Airplane>(airplaneCreateDto);
            // Check if the input DTO is null
            if (airplaneCreateDto == null)
            {
                throw new ArgumentNullException(nameof(airplaneCreateDto), "Airplane cannot be null.");
            }
            try
            {
                // Add airplane to repository and save changes
                _airplaneRepository.AddAirplane(airplane);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                throw new ApplicationException("An error occurred while adding the airplane.", ex);
            }
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
        public Airplane GetAirplaneByIdWithRelatedData(int id)
        {
            // Retrieve the airplane entity by ID
            var airplane = _airplaneRepository.GetById(id);

            if (airplane == null)
            {
                return null;
            }

            return airplane;
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

        public void UpdateAirplane(Airplane airplane)
        {
            // Update the airplane in the repository
            _airplaneRepository.Update(airplane);
        }

        // Delete an airplane's details
        public bool DeleteAirplane(int id)
        {
            var airplane = _airplaneRepository.GetById(id);

            if (airplane == null)
            {
                return false;  // Airplane not found
            }

            // Delete the airplane from the repository
            _airplaneRepository.Delete(airplane);

            return true;  // Successfully deleted
        }
    }
}
