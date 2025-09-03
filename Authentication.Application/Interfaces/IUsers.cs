using Authentication.Application.Dtos;
using Authentication.Domain.Entities;
using SharedLibrary.Resonse;

namespace Authentication.Application.Interfaces
{
    public interface IUsers
    {
        Task<ApiRespose> RegisterAsync(AppUserRequestDto appUser);
        Task<ApiRespose> Login(LoginDto appUser);
        Task<AppUserRequestDto> GetUserAsync(string userId);
        Task<ApiRespose> UpdateUserAsync(AppUserRequestDto userDto);
    }
}