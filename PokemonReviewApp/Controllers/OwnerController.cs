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
    public class OwnerController(IOwnerRepository ownerRepository,ICountryRepository countryRepository, IMapper mapper) : Controller
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<OwnerDto>))]
        public IActionResult GetOwners()
        {
            var owners = mapper.Map<List<OwnerDto>>(ownerRepository.GetOwners());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId)
        {
            if (!ownerRepository.OwnerExists(ownerId)) return NotFound();
            var owner = mapper.Map<OwnerDto>(ownerRepository.GetOwner(ownerId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemon", Name = "GetPokemonByOwner")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!ownerRepository.OwnerExists(ownerId)) return NotFound();
            var pokemon = mapper.Map<OwnerDto>(ownerRepository.GetPokemonByOwner(ownerId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId,[FromBody] OwnerDto OwnerCreate)
        {
            if (OwnerCreate == null) return BadRequest(ModelState);
            var owner = ownerRepository
                .GetOwners()
                .FirstOrDefault(c => c.LastName.Trim().ToUpper() == OwnerCreate.LastName.Trim().ToUpper());

            if (owner != null)
            {
                ModelState.AddModelError("", $"Owner {OwnerCreate.LastName} already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ownerMap = mapper.Map<Owner>(OwnerCreate);
            ownerMap.Country= countryRepository.GetCountry(countryId);
            if (!ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving {OwnerCreate.LastName}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }
    }
}