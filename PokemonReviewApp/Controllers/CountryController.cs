using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;
using System.Collections.Generic;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController(ICountryRepository _countryRepository, IMapper mapper) : Controller
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = mapper.Map<List<CountryDto>> (_countryRepository.GetCountries());
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(countries);
        }

        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId)) return NotFound();
            var country = mapper.Map<CountryDto> (_countryRepository.GetCountry(countryId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(country);
        }

        [HttpGet("owner/{ownerId}", Name = "GetCountryByOwner")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryByOwner(int ownerId)
        {
            if (!_countryRepository.CountryExists(ownerId)) return NotFound();
            var country = mapper.Map<CountryDto> (_countryRepository.GetCountryByOwner(ownerId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(country);
        }

        [HttpGet("pokemon/{countryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonsByCountryId(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId)) return NotFound();
            var pokemons = mapper.Map<CountryDto>(_countryRepository.GetOwnersByCountry(countryId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto CountryCreate)
        {
            if (CountryCreate == null) return BadRequest(ModelState);
            var country = _countryRepository
                .GetCountries()
                .FirstOrDefault(c => c.Name.Trim().ToUpper() == CountryCreate.Name.Trim().ToUpper());

            if (country != null)
            {
                ModelState.AddModelError("", $"Category {CountryCreate.Name} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var countryMap = mapper.Map<Country>(CountryCreate);
            if (!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving {CountryCreate.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }
    }
}
