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
    public class ReviewController(IReviewRepository reviewRepository, IMapper mapper, IPokemonRepository pokemonRepository, IReviewerRepository reviewer) : Controller
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

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokeId,[FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null)
                return BadRequest(ModelState);

            var reviews = reviewRepository.GetReviews()
                .FirstOrDefault(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper());

            if (reviews != null)
            {
                ModelState.AddModelError("", "pokemon already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = mapper.Map<Review>(reviewCreate);

            reviewMap.Pokemon = pokemonRepository.GetPokemon(pokeId);
            reviewMap.Reviewer = reviewer.GetReviewer(reviewerId);


            if (!reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

    }


}
