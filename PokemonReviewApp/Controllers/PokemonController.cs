using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository, IMapper mapper) : Controller
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = mapper.Map<List<PokemonDto>>(pokemonRepository.GetPokemons());
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}", Name = "GetPokemon")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!pokemonRepository.PokemonExists(pokeId)) return NotFound();
            var pokemon = mapper.Map<PokemonDto>(pokemonRepository.GetPokemon(pokeId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating", Name = "GetRating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if(!pokemonRepository.PokemonExists(pokeId)) return NotFound();
            var rating = pokemonRepository.GetPokemonRating(pokeId);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
                return BadRequest(ModelState);

            var pokemons = pokemonRepository.GetPokemons()
                .FirstOrDefault(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper());

            if (pokemons != null)
            {
                ModelState.AddModelError("", "pokemon already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = mapper.Map<Pokemon>(pokemonCreate);


            if (!pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int pokeId, [FromQuery] int ownerId, [FromQuery] int catId,[FromBody] PokemonDto pokemonUpdate)
        {
            if (pokemonUpdate == null) return BadRequest(ModelState);

            if (pokeId != pokemonUpdate.Id) return BadRequest(ModelState);

            if (!pokemonRepository.PokemonExists(pokeId)) return NotFound();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pokemonMap = mapper.Map<Pokemon>(pokemonUpdate);

            if (!pokemonRepository.UpdatePokemon(ownerId,catId,pokemonMap))
            {
                ModelState.AddModelError("", $"Something went wrong updating {pokemonUpdate.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!pokemonRepository.PokemonExists(pokeId)) return NotFound();

            var reviewsToDelete = reviewRepository.GetReviewsOfAPokemon(pokeId);
            var pokemonToDelete = pokemonRepository.GetPokemon(pokeId);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {reviewsToDelete}");
                return StatusCode(500, ModelState);
            }

            if (!pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {pokemonToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
