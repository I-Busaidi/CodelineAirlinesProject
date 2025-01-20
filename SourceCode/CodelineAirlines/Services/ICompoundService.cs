using CodelineAirlines.DTOs.AirplaneDTOs;
using CodelineAirlines.DTOs.AirplaneSpecDTOs;
using CodelineAirlines.DTOs.AirportDTOs;
using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.Enums;

namespace CodelineAirlines.Services
{
    public interface ICompoundService
    {
        int AddFlight(FlightInputDTO flightInput);

        (int flightNo, int BookingsCount) CancelFlight(int flightId, string condition);

        FlightDetailedOutputDTO GetFlightDetails(int flightNo);

        void AddReview(ReviewInputDTO review);

        public (int FlightNo, DateTime NewDepartureDate) RescheduleFlight(int flightNo, DateTime newDate, int airplaneId = -1);

        (int FlightNo, string? Status) Land(int flightNo);

        List<SeatsOutputDTO> GetAvailableSeats(int flightNo);

        (string airportName, string country, string city) AddAirport(AirportControllerInputDTO airportInput);

        int ClaculateFlightInputDuration(FlightControllerInput flightControllerInput);

        string AddAirplaneModel(AirplaneSpecInputDTO airplaneSpecInput);
    }
}