using System.ComponentModel.DataAnnotations;

namespace Products.Catalogue.Application.Dtos
{
    public record BrandDto(int Id,[Required] string Name);
}
