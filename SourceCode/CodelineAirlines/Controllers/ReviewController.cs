using AutoMapper;
using CodelineAirlines.DTOs.PassengerDTOs;
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
        private readonly IMapper _mapper;
        private readonly ICompoundService _compoundService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;

        }

        [HttpPost("AddReview")]
        public IActionResult AddReview([FromQuery] ReviewInputDTO reviewInput)
        {
            try
            {   
                
                //Check here after done-----------------------------------

                // Retrieve the current user's passport
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User ID is missing or invalid.");
                }
                // Map the input DTO to the Review 
                var newReview = _mapper.Map<Review>(reviewInput);

                // Set the ReviewerId (from the JWT token) on the Review entity
                newReview.Reviewer.UserId = int.Parse(userId);

                // Call the service method to add the review
                _compoundService.AddReview(reviewInput);

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
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
        [HttpGet("user-reviews")]
        public IActionResult GetAllUserReviews()
        {
            try
            {
                // Retrieve the current user's ID from JWT claims
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
             

                // Call the service to fetch the reviews
                var reviews = _reviewService.GetAllReviewsByUser(userId);

                return Ok(new { Message = "Reviews retrieved successfully.", Reviews = reviews });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the reviews.", Error = ex.Message });
            }
        }

        // Update an existing review
        [HttpPut("{reviewId}")]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewInputDTO review)
        {
            if (review == null || review.ReviewId != reviewId)
            {
                return BadRequest("Invalid review data.");
            }

            try
            {
                _reviewService.UpdateReview(review);
                return Ok("Review updated successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllFlighitsReviews()
        {
            try
            {
                var reviews = _reviewService.GetAllReview();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        // Delete a review
        [HttpDelete("{reviewId}")]
        public IActionResult DeleteReview(int reviewId)
        {
            try
            {
                _reviewService.DeleteReview(reviewId);
                return Ok("Review deleted successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

    }
}
