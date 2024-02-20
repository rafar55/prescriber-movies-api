using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesDb.Application.Users.Dtos;
using MoviesDb.Application.Users.Services;

namespace MoviesDb.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
        {
            var tokenResponse = await _userService.AuthenticateAsync(request);
            return tokenResponse;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserResponse>> Register([FromBody] CreateUserRequest request)
        {
            var newUser = await _userService.CreateAsync(request);
            return newUser;
        }


        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<UserResponse> Profile()
        {
            var user = await _userService.GetByEmailAsync(User.Identity!.Name!);
            return user;
        }
    }
}
