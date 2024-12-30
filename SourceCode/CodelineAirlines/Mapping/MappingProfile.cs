using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.DTOs.PassengerDTOs;
using CodelineAirlines.DTOs.ReviewDTOs;
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
            CreateMap<GenerateSeatTemplateDto, SeatTemplate>();
     
            CreateMap<Passenger, PassengerOutputDTO>();

            // Map from PassengerInputDTOs to Passenger
            CreateMap<PassengerInputDTOs, Passenger>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateOnly(src.BirthDate.Year, src.BirthDate.Month, src.BirthDate.Day)))
                .ForMember(dest => dest.Passport, opt => opt.MapFrom(src => src.Passport))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality));
            //Map from ReviewInputDTO to Review 
            CreateMap<ReviewInputDTO, Review>();

            CreateMap<FlightInputDTO, Flight>()
                .ForMember(dest => dest.SourceAirportId , opt => opt.MapFrom<SourceAirportNameResolver>())
                .ForMember(dest => dest.DestinationAirportId, opt => opt.MapFrom<DestinationAirportNameResolver>());
        }
    }
    
}
