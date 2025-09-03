using Mapster;
using Microsoft.AspNetCore.Mvc;
using Products.Catalogue.Application.Dtos;
using Products.Catalogue.Application.Interfaces;
using Products.Catalogue.Domain.Entities;
using SharedLibrary.Resonse;

namespace ProductAPi.PresentationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProduct _proRepo;
        private readonly ICategory _categoryRepo; 
        private readonly IBrand _brandRepo;      

        public ProductController(
            IProduct proRepo,
            ICategory categoryRepo,
            IBrand brandRepo)
        {
            _proRepo = proRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            // Use the method that INCLUDES category and brand details
            var products = await _proRepo.GetAllWithDetailsAsync();
            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }

            var productDtos = products.Adapt<List<ProductDto>>();
            return Ok(productDtos);
        }

        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            // Use the method that INCLUDES category and brand details
            var product = await _proRepo.GetByIdWithDetailsAsync(id);

            if (product == null)
            {
                return NotFound(new ApiRespose(false, $"Product with ID {id} not found."));
            }

            var productDto = product.Adapt<ProductDto>();
            return Ok(productDto);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<ApiRespose>> AddProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiRespose(false, "Validation failed."));
            }

            // Because the DTO has names, we must find the IDs before creating the entity.
            var category = await _categoryRepo.GetByAsync(c => c.Name == productDto.CategoryName);
            var brand = await _brandRepo.GetByAsync(b => b.Name == productDto.BrandName);

            if (category == null) return BadRequest(new ApiRespose(false, $"Category '{productDto.CategoryName}' not found."));
            if (brand == null) return BadRequest(new ApiRespose(false, $"Brand '{productDto.BrandName}' not found."));

            var product = productDto.Adapt<Product>();
            product.CategoryId = category.Id; // Set the correct foreign key
            product.BrandId = brand.Id;       // Set the correct foreign key

            var response = await _proRepo.AddAsync(product);

            if (!response.flags)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // PUT: api/product/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiRespose>> UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest(new ApiRespose(false, "Product ID mismatch."));
            }
            // (You would add the same logic here as in AddProduct to look up CategoryId and BrandId)

            var product = productDto.Adapt<Product>();
            // ... set CategoryId and BrandId like in AddProduct ...

            var response = await _proRepo.UpdateAsync(product);

            if (!response.flags)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiRespose>> DeleteProduct(int id)
        {
            // 1. Create a new Product entity instance.
            var productToDelete = new Product { Id = id };

            // 2. Pass this entity to the repository. The repository will use the Id to find and delete the record.
            var response = await _proRepo.DeleteAsync(productToDelete);

            if (!response.flags)
            {
                // The repository handles the "not found" message.
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}