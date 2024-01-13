using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        ICollection<Review> GetReviewsOfAPokemon(int pokeId);
        Review GetReview(int reviewId);
        bool ReviewExists(int reviewId);
    }
}
