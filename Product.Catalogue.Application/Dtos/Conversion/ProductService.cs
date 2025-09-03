using MapsterMapper;
using Products.Catalogue.Application.Dtos;
using Products.Catalogue.Domain.Entities;

public class ProductService
{
    private readonly IMapper _mapper;

    public ProductService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public ProductDto ConvertToDto(Product product) =>
        _mapper.Map<ProductDto>(product);

    public List<ProductDto> ConvertToDtoList(IEnumerable<Product> products) =>
        _mapper.Map<List<ProductDto>>(products);

    public Product ConvertToEntity(ProductDto dto) =>
        _mapper.Map<Product>(dto);
}
