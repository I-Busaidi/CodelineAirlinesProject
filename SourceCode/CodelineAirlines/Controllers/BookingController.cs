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
    }
}
