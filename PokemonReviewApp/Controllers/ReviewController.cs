using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IReviewRepository reviewRepository, IMapper mapper) : Controller
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetReviews()
        {
            var reviews = mapper.Map<List<ReviewDto>>(reviewRepository.GetReviews());
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(reviews);
        }

        [HttpGet("{reviewId}", Name = "GetReview")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!reviewRepository.ReviewExists(reviewId)) return NotFound();
            var review = mapper.Map<ReviewDto>(reviewRepository.GetReview(reviewId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(review);
        }

        [HttpGet("pokemon/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            if (!reviewRepository.ReviewExists(pokeId)) return NotFound();
            var reviews = mapper.Map<ReviewDto>(reviewRepository.GetReviewsOfAPokemon(pokeId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(reviews);
        }

    }
}
