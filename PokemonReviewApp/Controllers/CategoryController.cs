using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryRepository _categoryRepository, IMapper mapper) : Controller
    {
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(categories);
        }

        [HttpGet("{categoryId}", Name = "GetCategory")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId)) return NotFound();
            var category = mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonsByCategoryId(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId)) return NotFound();
            var pokemons = mapper.Map<List<PokemonDto>>(_categoryRepository.GetPokemonsByCategory(categoryId));
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if (categoryCreate == null) return BadRequest(ModelState);
            var category = _categoryRepository
                .GetCategories()
                .FirstOrDefault(c => c.Name.Trim().ToUpper() == categoryCreate.Name.Trim().ToUpper());

            if (category != null)
            {
                ModelState.AddModelError("", $"Category {categoryCreate.Name} already exists");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid) return BadRequest(ModelState);

            var categoryMap= mapper.Map<Category>(categoryCreate);
            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", $"Something went wrong saving {categoryCreate.Name}");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }
    }
}
