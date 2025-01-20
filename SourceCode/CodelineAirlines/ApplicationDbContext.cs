using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeatTemplate>()
                .HasIndex(st => new { st.AirplaneModel, st.SeatNumber })
                .IsUnique();

            modelBuilder.Entity<Airport>()
                .HasIndex(ap => ap.AirportName)
                .IsUnique();

            modelBuilder.Entity<Passenger>()
                .HasIndex(ps => ps.Passport)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(bk => new { bk.SeatNo, bk.FlightNo })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserEmail)
                .IsUnique();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<SeatTemplate> SeatTemplates { get; set; }
        public DbSet<AirplaneSpecs> AirplaneSpecs { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<AirportLocation> AirportLocations { get; set; }
    }
}
