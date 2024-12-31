using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.Models;

namespace CodelineAirlines.Services
{
    public interface IReviewService
    {
        void AddReview(ReviewInputDTO review);
        ReviewInputDTO UpdateReview(ReviewInputDTO updatedReview);
        List<Review> GetAllReview();
        void DeleteReview(int reviewId);
    }
}