using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.PassengerDTOs;
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
            CreateMap<Airplane, AirplaneOutputDto>()
                .ForMember(dest => dest.AirportName, opt => opt.MapFrom(src => src.Airport.AirportName)); // Mapping the Airport Name
            CreateMap<UserInputDTOs, User>()
              .ForMember(dest => dest.Password, opt => opt.Ignore()); // Ignore password by default
            CreateMap<Airport, AirportOutputDTO>();
            CreateMap<PassengerInputDTOs, Passenger>();
            CreateMap<Passenger, PassengerOutputDTO>();
        }
    }
}
