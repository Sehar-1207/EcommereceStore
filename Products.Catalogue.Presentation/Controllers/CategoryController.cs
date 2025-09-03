using Mapster;
using Microsoft.AspNetCore.Mvc;
using Products.Catalogue.Application.Dtos;
using Products.Catalogue.Application.Interfaces;
using Products.Catalogue.Domain.Entities;
using SharedLibrary.Resonse;

namespace Products.Catalogue.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryRepo;

        public CategoryController(ICategory categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryRepo.GetAllAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }
            var categoryDtos = categories.Adapt<List<CategoryDto>>();
            return Ok(categoryDtos);
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var response = await _categoryRepo.FindIdAsync(id);
            if (!response.flags || response.Data is not Category category)
            {
                return NotFound(response.Message);
            }
            var categoryDto = category.Adapt<CategoryDto>();
            return Ok(categoryDto);
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<ApiRespose>> AddCategory([FromBody] CategoryDto categoryDto)
        {
            var category = categoryDto.Adapt<Category>();
            var response = await _categoryRepo.AddAsync(category);
            if (!response.flags)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // PUT: api/category/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiRespose>> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest(new ApiRespose(false, "Category ID mismatch."));
            }
            var category = categoryDto.Adapt<Category>();
            var response = await _categoryRepo.UpdateAsync(category);
            if (!response.flags)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // DELETE: api/category/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiRespose>> DeleteCategory(int id)
        {
            // 1. Create a new Category entity with only the Id set.
            var categoryToDelete = new Category { Id = id };

            // 2. Pass the entity to the repository's DeleteAsync method.
            var response = await _categoryRepo.DeleteAsync(categoryToDelete);

            if (!response.flags)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}