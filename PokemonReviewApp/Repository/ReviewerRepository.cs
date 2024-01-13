using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class ReviewerRepository(DataContext context,IMapper mapper):IReviewerRepository
    {
        public ICollection<Reviewer> GetReviewers()
        {
            return context.Reviewer.ToList();
        }

        public Reviewer GetReviewer(int reviewerId)
        {
            return context.Reviewer.FirstOrDefault(r => r.Id == reviewerId);
        }

        public ICollection<Review> GetReviewsOfAReviewer(int reviewerId)
        {
            return context.Review.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return context.Reviewer.Any(r => r.Id == reviewerId);
        }
    }
}
