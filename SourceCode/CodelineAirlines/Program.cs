
using CodelineAirlines.Helpers.WeatherForecast;
using CodelineAirlines.Mapping;
using CodelineAirlines.Repositories;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CodelineAirlines
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Adding DB Context.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Adding Airport related services.
            builder.Services.AddScoped<IAirportRepository, AirportRepository>();
            builder.Services.AddScoped<IAirportService, AirportService>();
            builder.Services.AddScoped<IAirportLocationRepository, AirportLocationRepository>();
            builder.Services.AddScoped<IAirportLocationService, AirportLocationService>();

            // Adding User related services.
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService,UserService>();

            // Adding Passenger related services.
            builder.Services.AddScoped<IPassengerRepository,PassengerRepository>();
            builder.Services.AddScoped<IPassengerService, PassengerService>();
            
            // Adding Airplane related servcies.
            builder.Services.AddScoped<IAirplaneRepository, AirplaneRepository>();
            builder.Services.AddScoped<IAirplaneService, AirplaneService>();

            // Adding Airplane specs related services.
            builder.Services.AddScoped<IAirplaneSpecRepository, AirplaneSpecRepository>();
            builder.Services.AddScoped<IAirplaneSpecService, AirplaneSpecService>();

            // Adding Seats Template related services.
            builder.Services.AddScoped<ISeatTemplateRepository, SeatTemplateRepository>();
            builder.Services.AddScoped<ISeatTemplateService, SeatTemplateService>();

            // adding email service
            builder.Services.AddScoped<IEmailService, EmailService>();

            //adding review related services
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            //adding booking related services
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // Adding Flight related services.
            builder.Services.AddScoped<IFlightRepository, FlightRepository>();
            builder.Services.AddScoped<IFlightService, FlightService>();

            // Adding Compound services.
            builder.Services.AddScoped<ICompoundService, CompoundService>();

            //Adding automapper
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            //Value Resolvers for AutoMapper
            builder.Services.AddScoped<SourceAirportNameResolver>();
            builder.Services.AddScoped<DestinationAirportNameResolver>();


            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient<WeatherService>(); // Used for weather forecast.
            builder.Services.AddControllers();

            // Add JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // You can set this to true if you want to validate the issuer.
                    ValidateAudience = false, // You can set this to true if you want to validate the audience.
                    ValidateLifetime = true, // Ensures the token hasn't expired.
                    ValidateIssuerSigningKey = true, // Ensures the token is properly signed.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // Match with your token generation key.
                };
            });



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer <token>')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
