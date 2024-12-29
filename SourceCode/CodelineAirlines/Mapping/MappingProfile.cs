using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;
namespace CodelineAirlines.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<AirportInputDTO, Airport>();
            CreateMap<AirplaneCreateDTO, Airplane>();
            CreateMap<Airplane, AirplaneOutputDto>();
            CreateMap<UserInputDTOs, User>();   
            CreateMap<Airport, AirportOutputDTO>();
        }
    }
}
