﻿using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly ApplicationDbContext _context;

        public AirportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string AddAirport(Airport airport)
        {
            _context.Airports.Add(airport);
            _context.SaveChanges();
            return airport.AirportName;
        }

        public IEnumerable<Airport> GetAllAirports()
        {
            return _context.Airports;
        }

        public Airport GetAirportByName(string name)
        {
            return _context.Airports.FirstOrDefault(ap => ap.AirportName == name);
        }
    }
}
