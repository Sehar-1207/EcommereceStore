using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.Dtos
{
    public record LoginDto
    (
        [Required, MaxLength(50)]string Email,
        [Required, PasswordPropertyText, MinLength(6)] string Password,
        string? AdminKey);
}