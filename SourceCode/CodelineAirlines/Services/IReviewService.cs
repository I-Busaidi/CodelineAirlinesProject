using CodelineAirlines.DTOs.ReviewDTOs;

namespace CodelineAirlines.Services
{
    public interface IReviewService
    {
        void AddReview(ReviewInputDTO review);
     ReviewInputDTO UpdateReview(ReviewInputDTO updatedReview);
    }
}