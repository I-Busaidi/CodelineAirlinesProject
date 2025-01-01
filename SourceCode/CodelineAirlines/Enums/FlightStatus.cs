using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Swashbuckle.AspNetCore.Annotations;

namespace CodelineAirlines.Enums
{
    public enum FlightStatus
    {
        
        Scheduled = 0,
        OnTime = 1,
        Delayed = 2,
        InFlight = 3,
        Arrived = 4
    }
}
