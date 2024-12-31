using CodelineAirlines.Models;

namespace CodelineAirlines.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public string AddReview(Review review)
        {
            try
            {
                _context.Reviews.Add(review);
                _context.SaveChanges();
                return review.Comment; 
            }

            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }
        public void UpdateReview(Review updatedReview)
        {
            var existingReview = _context.Reviews.FirstOrDefault(r => r.ReviewId == updatedReview.ReviewId);
            if (existingReview != null)
            {
                existingReview.Rating = updatedReview.Rating;
                existingReview.Comment = updatedReview.Comment;
                _context.SaveChanges();
            }

        }
        public Review GetReviewById(int id)
        {
            return _context.Reviews.FirstOrDefault(p => p.ReviewId == id);
        }



    }
}
