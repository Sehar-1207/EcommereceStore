using Mapster;
using Products.Catalogue.Application.Dtos;
using Products.Catalogue.Domain.Entities;

namespace Products.Catalogue.Application.Common.Mappings
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // --- Category Mappings ---
            // Entity to DTO
            config.NewConfig<Category, CategoryDto>();
            // DTO to Entity (for creation/updates)
            config.NewConfig<CategoryDto, Category>()
                .Ignore(dest => dest.Products); // Avoid mapping collections back


            // --- Brand Mappings ---
            // Entity to DTO
            config.NewConfig<Brand, BrandDto>();
            // DTO to Entity (for creation/updates)
            config.NewConfig<BrandDto, Brand>()
                .Ignore(dest => dest.Products);


            // --- Product Mappings (Most Important) ---

            // Mapping from Product Entity -> ProductDto (For Reading Data)
            config.NewConfig<Product, ProductDto>()
                .Map(dest => dest.CategoryName,
                     src => src.Category != null ? src.Category.Name : null) // Safely map the category name
                .Map(dest => dest.BrandName,
                     src => src.Brand != null ? src.Brand.Name : null);    // Safely map the brand name


            // Mapping from a DTO -> Product Entity (For Creating/Updating Data)Assuming you'll use a DTO that includes CategoryId and BrandId for creation
            // I'll provide mappings for both the general ProductDto and the recommended CreateProductDto

            // Mapping from the *recommended* CreateProductDto
            //config.NewConfig<CreateProductDto, Product>();

            // If you insist on using ProductDto for creation (not recommended), the mapping is more complex
            // and you'd have to load the Category/Brand entities separately in your service. This mapping ignores the complex parts.
            config.NewConfig<ProductDto, Product>()
                .Ignore(dest => dest.Id)         // Don't map Id when creating
                .Ignore(dest => dest.Category)   // Ignore navigation properties
                .Ignore(dest => dest.Brand);      // to prevent issues with Entity Framework

        }
    }
}