using System.ComponentModel.DataAnnotations;

namespace Products.Catalogue.Application.Dtos
{
   public record ProductDto(
       int Id,
      [Required] string Name, 
      string? Description,
      [Required, DataType(DataType.Currency)]decimal Price,
      [Required, Range(1, int.MaxValue)]int Quantity,
      string? PictureUrl,
      [Required]string CategoryName,
      [Required]string BrandName
       );
}
