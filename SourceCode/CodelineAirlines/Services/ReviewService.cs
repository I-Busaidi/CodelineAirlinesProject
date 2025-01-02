using AutoMapper;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Enums;
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
       
        public void AddReview(Review review)
        {


            if (review == null)
            {
                throw new ArgumentNullException(nameof(review), "Review cannot be null.");
            }

            // Add the review to the repository
            _reviewRepository.AddReview(review);
        }

        // Helper method to get the current authenticated user's ID
        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public IEnumerable<ReviewInputDTO> GetAllReviewsByUser(int userId)
        {
         try
         {
                // Fetch all reviews for the user
               var reviews = _reviewRepository.GetReviewsByUserId(userId);

                if (!reviews.Any())
                  {
                       throw new KeyNotFoundException("No reviews found for the current user.");
                 }

                    // Map the reviews to DTOs
                      return reviews.Select(review => new ReviewInputDTO
                         {
                      ReviewId = review.ReviewId,
                      Rating = review.Rating,
                      Comment = review.Comment,
                      ReviewerPassport = review.Reviewer.Passport,
                      FlightNo = review.FlightReview.FlightNo
                        });
         }
            catch (KeyNotFoundException ex)
                {
               throw new ApplicationException("Error: " + ex.Message, ex);
                }
            catch (Exception ex)
            {
                 throw new ApplicationException("An unexpected error occurred: " + ex.Message, ex);
            }
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
                // Check if the Reviewer property is loaded
                if (existingReview.Reviewer == null)
                {
                    throw new InvalidOperationException("Reviewer information is missing for the review.");
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
       public List<Review> GetAllReview()
        {

            return _reviewRepository.GetAllReview();
        }

        // Delete a review (only by the creator)
        public void DeleteReview(int reviewId)
        {
            int userId = GetCurrentUserId();
            var existingReview = _reviewRepository.GetReviewById(reviewId);

            // Ensure the user is authorized to Delete their own review
            if (existingReview.Reviewer.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can only Delete your own reviews.");
            }


            _reviewRepository.DeleteReview(reviewId);

      
        }


    }
}
