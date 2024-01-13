using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper) : Controller
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = mapper.Map<List<ReviewerDto>>(reviewerRepository.GetReviewers());
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}", Name = "GetReviewer")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!reviewerRepository.ReviewerExists(reviewerId)) return NotFound();
            var reviewer = mapper.Map<ReviewerDto>(reviewerRepository.GetReviewer(reviewerId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews", Name = "GetReviewsByReviewer")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!reviewerRepository.ReviewerExists(reviewerId)) return NotFound();
            var reviews = mapper.Map<ReviewerDto>(reviewerRepository.GetReviewsOfAReviewer(reviewerId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(reviews);
        }
    }
}
