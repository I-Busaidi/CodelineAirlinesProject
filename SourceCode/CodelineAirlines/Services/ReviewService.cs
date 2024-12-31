using AutoMapper;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CodelineAirlines.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddReview(ReviewInputDTO review)
        {
            //Add condition "Review can be added after flights arrived only"--------------
            Review Newreview = _mapper.Map<Review>(review);

            _reviewRepository.AddReview(Newreview);

        }
        // Helper method to get the current authenticated user's ID
        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        // Update an existing review (only by the creator)
        public ReviewInputDTO UpdateReview(ReviewInputDTO updatedReview)
        {
            try
            {
                // Validate that ReviewId is provided
                if (!updatedReview.ReviewId.HasValue)
                {
                    throw new ArgumentException("Review ID is required for updating a review.");
                }

                // Retrieve the user ID 
                int userId = GetCurrentUserId();
                

                // Fetch the existing review by ID
                var existingReview = _reviewRepository.GetReviewById(updatedReview.ReviewId.Value);
                if (existingReview == null)
                {
                    throw new KeyNotFoundException("The review does not exist.");
                }

                // Ensure the user is authorized to update their own review
                if (existingReview.Reviewer.UserId != userId) 
                {
                    throw new UnauthorizedAccessException("You can only update your own reviews.");
                }

                // Update the review properties
                existingReview.Rating = updatedReview.Rating;
                existingReview.Comment = updatedReview.Comment;

                // Save changes in the repository
                _reviewRepository.UpdateReview(existingReview);

                // Return the updated review as DTO
                return new ReviewInputDTO
                {
                    ReviewId = existingReview.ReviewId,
                    Rating = existingReview.Rating,
                    Comment = existingReview.Comment
                };
            }
            catch (ArgumentException ex)
            {
                // Handle validation exceptions
                throw new ApplicationException("Invalid input: " + ex.Message, ex);
            }
            catch (KeyNotFoundException ex)
            {
                // Handle missing resource exceptions
                throw new ApplicationException("Error: " + ex.Message, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle authorization exceptions
                throw new ApplicationException("Access Denied: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                throw new ApplicationException("An unexpected error occurred: " + ex.Message, ex);
            }
        }
    }
}
