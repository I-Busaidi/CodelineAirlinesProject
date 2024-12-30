﻿using CodelineAirlines.DTOs.PassengerDTOs;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodelineAirlines.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class ReviewController:ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;

        }

        [HttpPost("AddReview")]
        public IActionResult AddPassenger([FromQuery] ReviewInputDTO reviewInput)
        {
            try
            {
                // Retrieve the current user's passport
                var reviewerPassport = reviewInput.ReviewerPassport;

                // Call the service method to add the Review
                _reviewService.AddReview(reviewInput);

                return Ok(new { Message = "Review created successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the passenger profile.", Error = ex.Message });
            }
        }
    }
}