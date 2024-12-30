using CodelineAirlines.DTOs.BookingDTOs;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodelineAirlines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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
                    return Ok("Flight booked successfully.");
                }

                return BadRequest("Failed to book the flight.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                    return Ok("Booking updated successfully.");
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
                    return Ok("Booking canceled successfully.");
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
