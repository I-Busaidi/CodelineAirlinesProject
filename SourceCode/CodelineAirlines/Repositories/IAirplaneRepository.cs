﻿using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IAirplaneRepository
    {
        void AddAirplane(Airplane airplane); // Method to add a new Airplane
        Airplane GetById(int id); // Method to get an airplane by ID
    }
}