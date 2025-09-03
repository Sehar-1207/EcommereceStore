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
    public class BrandController : ControllerBase
    {
        private readonly IBrand _brandRepo;

        public BrandController(IBrand brandRepo)
        {
            _brandRepo = brandRepo;
        }

        // GET: api/brand
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAllBrands()
        {
            var brands = await _brandRepo.GetAllAsync();
            if (brands == null || !brands.Any())
            {
                return NotFound("No brands found.");
            }
            var brandDtos = brands.Adapt<List<BrandDto>>();
            return Ok(brandDtos);
        }

        // GET: api/brand/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDto>> GetBrandById(int id)
        {
            var response = await _brandRepo.FindIdAsync(id);
            if (!response.flags || response.Data is not Brand brand)
            {
                return NotFound(response.Message);
            }
            var brandDto = brand.Adapt<BrandDto>();
            return Ok(brandDto);
        }

        // POST: api/brand
        [HttpPost]
        public async Task<ActionResult<ApiRespose>> AddBrand([FromBody] BrandDto brandDto)
        {
            var brand = brandDto.Adapt<Brand>();
            var response = await _brandRepo.AddAsync(brand);
            if (!response.flags)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // PUT: api/brand/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiRespose>> UpdateBrand(int id, [FromBody] BrandDto brandDto)
        {
            if (id != brandDto.Id)
            {
                return BadRequest(new ApiRespose(false, "Brand ID mismatch."));
            }
            var brand = brandDto.Adapt<Brand>();
            var response = await _brandRepo.UpdateAsync(brand);
            if (!response.flags)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // DELETE: api/brand/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiRespose>> DeleteBrand(int id)
        {
            // 1. Create a new Brand entity with only the Id set.
            var brandToDelete = new Brand { Id = id };

            // 2. Pass the entity to the repository's DeleteAsync method.
            var response = await _brandRepo.DeleteAsync(brandToDelete);

            if (!response.flags)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}