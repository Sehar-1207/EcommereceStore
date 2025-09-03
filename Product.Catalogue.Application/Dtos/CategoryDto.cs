using System.ComponentModel.DataAnnotations;

namespace Products.Catalogue.Application.Dtos
{
    public record CategoryDto(
        int Id,
        [Required]string Name,
        string? Description
         );
}
