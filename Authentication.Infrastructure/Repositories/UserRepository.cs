using Authentication.Application.Dtos;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Logs;
using SharedLibrary.Resonse;

namespace Authentication.Infrastructure.Data.Repositories
{
    public class UserRepository : IUsers
    {
        private readonly UserManager<Appuser> _userManager;
        private readonly SignInManager<Appuser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IToken _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration; // ✅ For AdminKey

        public UserRepository(UserManager<Appuser> userManager,
            SignInManager<Appuser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IToken tokenService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _configuration = configuration;
        }

        // ✅ Register with AdminKey role check
        public async Task<ApiRespose> RegisterAsync(AppUserRequestDto userDto)
        {
            try
            {
                var userByEmail = await _userManager.FindByEmailAsync(userDto.Email);
                if (userByEmail != null)
                    return new ApiRespose(false, $"User with email '{userDto.Email}' already exists.");

                var userByUsername = await _userManager.FindByNameAsync(userDto.Username);
                if (userByUsername != null)
                    return new ApiRespose(false, $"Username '{userDto.Username}' is already taken.");

                var storedAdminKey = _configuration["AdminSettings:AdminKey"];
                string roleToAssign = userDto.Role;

                if (!string.IsNullOrEmpty(userDto.AdminKey) && userDto.AdminKey == storedAdminKey)
                {
                    roleToAssign = "Admin";
                }

                if (!await _roleManager.RoleExistsAsync(roleToAssign))
                    return new ApiRespose(false, $"The role '{roleToAssign}' does not exist.");

                var userToCreate = _mapper.Map<Appuser>(userDto);
                var result = await _userManager.CreateAsync(userToCreate, userDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ApiRespose(false, $"User creation failed: {errors}");
                }

                await _userManager.AddToRoleAsync(userToCreate, roleToAssign);

                var token = await _tokenService.CreateAccessTokenAsync(userToCreate);
                var responseDto = new AuthenticatedUserDto(userToCreate.Fullname, userToCreate.Email, token);

                return new ApiRespose(true, "User registered successfully!", responseDto);
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred during user registration.");
            }
        }


        // ✅ Login with AdminKey role check
        public async Task<ApiRespose> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                    return new ApiRespose(false, "Invalid email or password.");

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var storedAdminKey = _configuration["AdminSettings:AdminKey"];
                    if (!string.IsNullOrEmpty(loginDto.AdminKey) && loginDto.AdminKey == storedAdminKey)
                    {
                        if (!await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }
                    }


                    var token = await _tokenService.CreateAccessTokenAsync(user);
                    var responseDto = new AuthenticatedUserDto(user.Fullname, user.Email, token);

                    return new ApiRespose(true, "Login successful.", responseDto);
                }

                return new ApiRespose(false, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred during login.");
            }
        }


        // ✅ Get User
        public async Task<AppUserRequestDto> GetUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return null;

                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault();
                var userDto = _mapper.Map<AppUserRequestDto>(user);

                return userDto with { Role = userRole };
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return null;
            }
        }

        // ✅ Update User
        public async Task<ApiRespose> UpdateUserAsync(AppUserRequestDto userDto)
        {
            try
            {
                var userToUpdate = await _userManager.FindByIdAsync(userDto.Id);
                if (userToUpdate == null)
                {
                    return new ApiRespose(false, $"User with ID '{userDto.Id}' not found.");
                }

                _mapper.Map(userDto, userToUpdate);
                var result = await _userManager.UpdateAsync(userToUpdate);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new ApiRespose(false, $"User update failed: {errors}");
                }

                return new ApiRespose(true, "User updated successfully.");
            }
            catch (Exception ex)
            {
                LogsException.LogException(ex);
                return new ApiRespose(false, "An error occurred while updating the user.");
            }
        }
    }
}
