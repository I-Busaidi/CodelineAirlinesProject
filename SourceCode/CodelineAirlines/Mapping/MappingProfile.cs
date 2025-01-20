using AutoMapper;
using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirplaneSpecDTOs;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.DTOs.PassengerDTOs;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Enums;
using CodelineAirlines.Models;
namespace CodelineAirlines.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            // Airport related maps
            CreateMap<AirportInputDTO, Airport>();
            CreateMap<Airport, AirportOutputDTO>();
            CreateMap<AirportLocation, AirportLocationOutputDTO>();

            // Airplane related maps
            CreateMap<AirplaneCreateDTO, Airplane>();
            CreateMap<Airplane, AirplaneOutputDto>()
                .ForMember(dest => dest.AirportName, opt => opt.MapFrom(src => src.Airport.AirportName)); // Mapping the Airport Name
            CreateMap<SeatTemplate, SeatsOutputDTO>()
                .ForMember(dest => dest.SeatLocation, opt => opt.MapFrom(src => GetSeatLocation(src.SeatLocation)));
            CreateMap<GenerateSeatTemplateDto, SeatTemplate>();

            CreateMap<AirplaneSpecs, AirplaneSpecOutputDTO>();

            // User related maps
            CreateMap<UserInputDTOs, User>()
              .ForMember(dest => dest.Password, opt => opt.Ignore()); // Ignore password by default

            // Passenger related maps
            CreateMap<Passenger, PassengerOutputDTO>();
            // Map from PassengerInputDTOs to Passenger
            CreateMap<PassengerInputDTOs, Passenger>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => new DateOnly(src.BirthDate.Year, src.BirthDate.Month, src.BirthDate.Day)))
                .ForMember(dest => dest.Passport, opt => opt.MapFrom(src => src.Passport))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality));

            //Map from ReviewInputDTO to Review 
            CreateMap<ReviewInputDTO, Review>();

            // Flight related maps
            CreateMap<FlightInputDTO, Flight>()
                .ForMember(dest => dest.SourceAirportId , opt => opt.MapFrom<SourceAirportNameResolver>())
                .ForMember(dest => dest.DestinationAirportId, opt => opt.MapFrom<DestinationAirportNameResolver>());
            CreateMap<Flight, FlightOutputDTO>()
                .ForMember(dest => dest.SourceAirportName, opt => opt.MapFrom(src => src.SourceAirport.AirportName))
                .ForMember(dest => dest.DestinationAirportName, opt => opt.MapFrom(src => src.DestinationAirport.AirportName))
                .ForMember(dest => dest.AirplaneModel, opt => opt.MapFrom(src => src.Airplane.AirplaneModel))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(FlightStatus), src.StatusCode)));
        }

        private string GetSeatLocation(int seatLocation)
        {
            if (seatLocation == 0)
            {
                return "Window seat";
            }
            else if (seatLocation == 1)
            {
                return "Aisle seat";
            }
            else
            {
                return "Middle seat";
            }
        }
    }
    
}
