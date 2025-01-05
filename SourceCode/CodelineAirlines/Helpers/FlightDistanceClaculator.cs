namespace CodelineAirlines.Helpers
{
    public class FlightDistanceClaculator
    {
        private const double EarthRadius = 6371.0;
        public static double ToRadians(double degree)
        {
            return degree * Math.PI / 180.0;
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1Rad = ToRadians(lat1);
            double lon1Rad = ToRadians(lon1);
            double lat2Rad = ToRadians(lat2);
            double lon2Rad = ToRadians(lon2);

            // Haversine formula
            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Distance in kilometers
            return EarthRadius * c;
        }

        public static TimeSpan CalculateFlightDuration(double distance, double speedKmPerHr)
        {
            // Calculate time in hours
            double hours = distance / speedKmPerHr;

            // Convert hours to TimeSpan
            TimeSpan flightTime = TimeSpan.FromHours(hours);

            // Round up to the nearest 15 minutes
            double roundedMinutes = Math.Ceiling(flightTime.TotalMinutes / 15) * 15;
            return TimeSpan.FromMinutes(roundedMinutes);
        }
    }
}
