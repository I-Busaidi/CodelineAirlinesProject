using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ICompoundService _compoundService;

        public BookingController(IBookingService bookingService, ICompoundService compoundService)
        {
            _bookingService = bookingService;
            _compoundService = compoundService;
        }

        [HttpPost]
        [Route("book-flight")]
        public IActionResult BookFlight([FromBody] BookingDTO bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest("Booking data is required.");
            }

            try
            {
                var result = _bookingService.BookFlight(bookingDto);

                if (result)
                {
                    return Ok("Flight booked successfully and confirmation email sent.");
                }

                return BadRequest("Failed to book the flight.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetFlightAvailableSeats/{flightNo}")]
        public IActionResult GetFlightAvailableSeats(int flightNo, string seatClass = "", string seatLocation = "")
        {
            try
            {
                var seats = _compoundService.GetAvailableSeats(flightNo).Where(s => s.SeatLocation.Contains(seatLocation)
                && s.Type.Contains(seatClass));

                return Ok(seats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get all bookings for admin
        [Authorize(Roles = "admin")]
        [HttpGet("admin/bookings")]
        public IEnumerable<Booking> GetAllBookingsForAdmin()
        {
            // Admins can see all bookings
            return _bookingService.GetAllBookingsForAdmin();
        }

        [HttpGet("passenger/bookings")]
        public IEnumerable<Booking> GetBookingsForPassenger(string passport)
        {
            // Ensure passport is provided for passengers
            if (string.IsNullOrEmpty(passport))
            {
                throw new Exception("Passport is required for passenger.");
            }

            // Passengers can only see their own bookings
            return _bookingService.GetBookingsForPassenger(passport);
        }


        // Update booking endpoint
        [HttpPut]
        [Route("update-booking")]
        public IActionResult UpdateBooking([FromBody] UpdateBookingDTO bookingDto)
        {
            if (bookingDto == null)
            {
                return BadRequest("Booking data is required.");
            }

            try
            {
                var result = _bookingService.UpdateBooking(bookingDto);

                if (result)
                {
                    return Ok("Booking updated successfully and email notification sent.");
                }

                return BadRequest("Failed to update the booking.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Cancel booking endpoint
        [HttpDelete]
        [Route("cancel-booking/{bookingId}")]
        public IActionResult CancelBooking(int bookingId)
        {
            try
            {
                var result = _bookingService.CancelBooking(bookingId);
                if (result)
                {
                    return Ok("Booking canceled successfully and email notification sent.");
                }

                return BadRequest("Failed to cancel the booking.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
