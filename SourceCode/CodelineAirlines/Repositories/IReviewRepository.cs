using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IReviewRepository
    {
        string AddReview(Review review);
        Review GetReviewById(int id);
        void UpdateReview(Review updatedReview);
        List<Review> GetAllReview();
        void DeleteReview(int id);
        IEnumerable<Review> GetReviewsByUserId(int userId);
    }
}