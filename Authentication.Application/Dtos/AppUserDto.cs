using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.Dtos
{
    public record AppUserRequestDto(
        [Required]string Id,
        [Required, MaxLength(50)]string Username,
        [Required, MaxLength(50)] string FullName,
        [Required, PasswordPropertyText,MinLength(6)]string Password,
        [Required,EmailAddress]string Email,
        [Required, MaxLength(100)] string Address,
        [Required, Phone]string PhoneNo,
        [Required] string Role,
        string? AdminKey
        );
    
}
