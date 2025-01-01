using AutoMapper;
using CodelineAirlines.DTOs.PassengerDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CodelineAirlines.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IPassengerRepository _passengerRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;


        public PassengerService(IPassengerRepository passengerRepository, IMapper mapper, IUserRepository userRepository)
        {
            _passengerRepository = passengerRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public void AddPassenger(PassengerInputDTOs passengerInputDTO, int userId, bool isAdmin)
        {
            // Check if user exists
            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // Ensure that only admins or the associated user can create the profile
            if (!isAdmin && user.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to create a passenger profile for this user.");
            }

            // Check if the user already has a passenger profile
            if (_passengerRepository.PassengerExistsForUser(userId))
            {
                throw new InvalidOperationException("A passenger profile already exists for this user.");
            }

            // Map the DTO to the Passenger entity
            var passenger = _mapper.Map<Passenger>(passengerInputDTO);
            passenger.UserId = userId;

            // Save to database
            _passengerRepository.AddPassenger(passenger);
        }

        public PassengerOutputDTO GetPassengerProfile(int userId)
        {
            // Get the passenger from the repository
            var passenger = _passengerRepository.GetPassengerByUserId(userId);

            // Map the Passenger entity to the PassengerDTO
            var passengerDTO = _mapper.Map<PassengerOutputDTO>(passenger);

            return passengerDTO;
        }
        public void UpdatePassengerDetails(int userId, PassengerInputDTOs passengerInputDTO)
        {
            // Map the DTO to the Passenger entity
            var passenger = _mapper.Map<Passenger>(passengerInputDTO);
            passenger.UserId = userId;  // Ensure that the UserId is set correctly

            // Update passenger details
            _passengerRepository.UpdatePassenger(passenger);
        }
        public int GetLoyaltyPoints(int userId)
        {
  
            return _passengerRepository.GetLoyaltyPointsByUserId(userId);
        }
        public Passenger GetPassengerByPassport(string passport)
        {
            if (string.IsNullOrEmpty(passport))
            {
                throw new ArgumentException("Passport number cannot be null or empty.", nameof(passport));
            }

            // Retrieve the passenger from the repository
            var passenger = _passengerRepository.GetByPassport(passport);

            if (passenger == null)
            {
                throw new KeyNotFoundException($"Passenger with passport {passport} not found.");
            }

            return passenger;
        }

    }
}
