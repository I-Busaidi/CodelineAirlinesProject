using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IReviewRepository
    {
        string AddReview(Review review);
        Review GetReviewById(int id);
        void UpdateReview(Review updatedReview);
    }
}