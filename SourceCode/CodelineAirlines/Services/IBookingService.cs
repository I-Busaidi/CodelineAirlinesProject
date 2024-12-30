﻿using CodelineAirlines.DTOs.BookingDTOs;

namespace CodelineAirlines.Services
{
    public interface IBookingService
    {
        bool BookFlight(BookingDTO bookingDto);
        bool CancelBooking(int bookingId);
        bool UpdateBooking(UpdateBookingDTO bookingDto);
    }
}