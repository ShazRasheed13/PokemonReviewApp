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

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            if (reviewerCreate == null) return BadRequest(ModelState);
            var reviewer = reviewerRepository
                .GetReviewers()
                .FirstOrDefault(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.Trim().ToUpper());

            if (reviewer != null)
            {
                ModelState.AddModelError("", $"Category {reviewerCreate.LastName} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reviewerMap = mapper.Map<Reviewer>(reviewerCreate);
            if (!reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving {reviewerCreate.LastName}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int reviewerId, [FromBody] ReviewerDto reviewerUpdate)
        {
            if (reviewerUpdate == null) return BadRequest(ModelState);

            if (reviewerId != reviewerUpdate.Id) return BadRequest(ModelState);

            if (!reviewerRepository.ReviewerExists(reviewerId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reviewerMap = mapper.Map<Reviewer>(reviewerUpdate);

            if (!reviewerRepository.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating {reviewerUpdate.FirstName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!reviewerRepository.ReviewerExists(reviewerId)) return NotFound();

            var reviewToDelete = reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!reviewerRepository.DeleteReviewer(reviewToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {reviewToDelete.FirstName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
