using CodelineAirlines.DTOs.FlightDTOs;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.Enums;

namespace CodelineAirlines.Services
{
    public interface ICompoundService
    {
        int AddFlight(FlightInputDTO flightInput);
        (int, string) UpdateFlightStatus(int flightId, FlightStatus flightStatus);

        (int flightNo, int BookingsCount) CancelFlight(int flightId, string condition);

        FlightDetailedOutputDTO GetFlightDetails(int flightNo);

        void AddReview(ReviewInputDTO review);

        public (int FlightNo, DateTime NewDepartureDate) RescheduleFlight(int flightNo, DateTime newDate, int airplaneId = -1);
    }
}