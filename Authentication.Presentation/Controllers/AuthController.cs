using Authentication.Application.Dtos;
using Authentication.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Resonse;
using System.Security.Claims;

namespace Authentication.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsers _usersService;

        public UsersController(IUsers usersService)
        {
            _usersService = usersService;
        }

        // --- AUTHENTICATION ENDPOINTS (Public) ---

        /// Registers a new user and returns their information along with a JWT.
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiRespose), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiRespose), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] AppUserRequestDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiRespose(false, "Invalid user data provided.", ModelState));
            }

            var response = await _usersService.RegisterAsync(registerDto);

            if (!response.flags)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// Authenticates a user and returns their information along with a JWT.
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiRespose), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiRespose), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiRespose(false, "Email and password are required.", ModelState));
            }

            var response = await _usersService.Login(loginDto);

            if (!response.flags)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        // --- USER MANAGEMENT ENDPOINTS (Protected) ---

        /// Gets the details of the currently logged-in user.
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(AppUserRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userDto = await _usersService.GetUserAsync(userId);
            if (userDto == null)
            {
                return NotFound(new ApiRespose(false, "User not found."));
            }

            return Ok(userDto);
        }

        /// Gets a specific user by their ID. (Admin Only)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AppUserRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            var userDto = await _usersService.GetUserAsync(id);
            if (userDto == null)
            {
                return NotFound(new ApiRespose(false, $"User with ID '{id}' not found."));
            }

            return Ok(userDto);
        }

        /// Updates a user's profile.
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiRespose), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiRespose), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] AppUserRequestDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest(new ApiRespose(false, "ID in URL does not match ID in body."));
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (currentUserId != id && !isAdmin)
            {
                return Forbid();
            }

            var response = await _usersService.UpdateUserAsync(userDto);

            if (!response.flags)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
