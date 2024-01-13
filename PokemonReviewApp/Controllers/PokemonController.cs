﻿using AutoMapper;
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
    public class PokemonController(IPokemonRepository pokemonRepository, IMapper mapper) : Controller
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

            var pokemons = pokemonRepository.GetPokemonTrimToUpper(pokemonCreate);

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Owner already exists");
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
    }
}
