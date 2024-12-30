using AutoMapper;
using CodelineAirlines.DTOs.ReviewDTOs;
using CodelineAirlines.DTOs.UserDTOs;
using CodelineAirlines.Models;
using CodelineAirlines.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace CodelineAirlines.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public void AddReview(ReviewInputDTO review)
        {

            Review Newreview = _mapper.Map<Review>(review);

            _reviewRepository.AddReview(Newreview);

        }
    }
}
