﻿using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IBookingRepository
    {
        void AddBooking(Booking booking);
        void CancelBooking(int bookingId);
        IEnumerable<Booking> GetAllBookings();
        Booking GetBookingById(int bookingId);
        IEnumerable<Booking> GetBookingsByPassenger(string passengerPassport);
        void UpdateBooking(Booking booking);
    }
}