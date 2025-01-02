using CodelineAirlines.Models;
using Microsoft.EntityFrameworkCore;

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
        public IEnumerable<Review> GetReviewsByUserId(int userId)
        {
            return _context.Reviews.Include(r => r.Reviewer)     // Include navigation properties
                .Include(r => r.FlightReview).Where(r => r.Reviewer.UserId == userId).ToList();
        }

        public Review GetReviewById(int id)
        {
            return _context.Reviews
                   .Include(r => r.Reviewer) // Include the Reviewer navigation property
                   .FirstOrDefault(r => r.ReviewId == id);
      
        }
        public List<Review> GetAllReview()
        {
            return _context.Reviews.ToList();

        }
        //delete review 
        public void DeleteReview(int id)
        {
            var review = _context.Reviews.FirstOrDefault(p => p.ReviewId == id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }

    }
}
