using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public interface IReviewRepository
    {
        string AddReview(Review review);
    }
}