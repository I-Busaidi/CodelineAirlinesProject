using AutoMapper;
using CodelineAirlines.DTOs.AirplaneSpecDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;

namespace CodelineAirlines.Services
{
    public class AirplaneSpecService : IAirplaneSpecService
    {
        private readonly IAirplaneSpecRepository _airplaneSpecRepository;
        private readonly IMapper _mapper;

        public AirplaneSpecService(IAirplaneSpecRepository airplaneSpecRepository, IMapper mapper)
        {
            _airplaneSpecRepository = airplaneSpecRepository;
            _mapper = mapper;
        }

        public AirplaneSpecs AddAirplaneSpecs(AirplaneSpecInputDTO airplaneSpecInputDTO)
        {
            if (string.IsNullOrWhiteSpace(airplaneSpecInputDTO.Model))
            {
                throw new ArgumentException("Model name is required");
            }

            var model = _airplaneSpecRepository.GetAirplaneSpecsByModel(airplaneSpecInputDTO.Model);

            if (model != null)
            {
                throw new InvalidOperationException("A model with this name already exists.");
            }

            if (airplaneSpecInputDTO.AvgSpeed < 500)
            {
                throw new ArgumentOutOfRangeException("Average airplane speed cannot be lower than 500 km/h");
            }

            if (airplaneSpecInputDTO.LuggageCapacity < 1000)
            {
                throw new ArgumentOutOfRangeException("Luggage capacity cannot be lower than 1000 kilos");
            }

            if (airplaneSpecInputDTO.LuggageCapacity > 6000)
            {
                throw new ArgumentOutOfRangeException("Luggage capacity cannot be higher than 6000 kilos");
            }

            int passengerCapacity = airplaneSpecInputDTO.SeatTemplate.FirstClassSeats
                + airplaneSpecInputDTO.SeatTemplate.BusinessSeats
                + airplaneSpecInputDTO.SeatTemplate.EconomySeats;

            var airplaneSpec = _airplaneSpecRepository.AddAirplaneSpecs(new AirplaneSpecs
            {
                Model = airplaneSpecInputDTO.Model,
                AvgSpeed = airplaneSpecInputDTO.AvgSpeed,
                LuggageCapacity = airplaneSpecInputDTO.LuggageCapacity,
                PassengerCapacity = passengerCapacity
            });

            return airplaneSpec;
        }

        public List<AirplaneSpecs> GetModelsSpecsWithRelatedData()
        {
            var models = _airplaneSpecRepository.GetAirplaneModelsSpecs().ToList();
            if (models == null || models.Count == 0)
            {
                throw new InvalidOperationException("No models found.");
            }
            return models;
        }

        public List<AirplaneSpecOutputDTO> GetModelsSpecs()
        {
            var models = _airplaneSpecRepository.GetAirplaneModelsSpecs().ToList();
            if (models == null || models.Count == 0)
            {
                throw new InvalidOperationException("No models found.");
            }
            return _mapper.Map<List<AirplaneSpecOutputDTO>>(models);
        }

        public AirplaneSpecs GetSpecsByModelWithRelatedData(string modelName)
        {
            var model = _airplaneSpecRepository.GetAirplaneSpecsByModel(modelName);
            if (model == null)
            {
                throw new KeyNotFoundException("Could not find a model with this name.");
            }

            return model;
        }

        public AirplaneSpecOutputDTO GetSpecsByModel(string modelName)
        {
            var model = _airplaneSpecRepository.GetAirplaneSpecsByModel(modelName);
            if (model == null)
            {
                throw new KeyNotFoundException("Could not find a model with this name.");
            }

            return _mapper.Map<AirplaneSpecOutputDTO>(model);
        }
    }
}
