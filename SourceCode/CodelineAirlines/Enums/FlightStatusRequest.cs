using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Annotations;

namespace CodelineAirlines.Enums
{
    public class FlightStatusRequest
    {
        public FlightStatus FlightStatus { get; set; }
    }
}
