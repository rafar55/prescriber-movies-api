using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesDb.Application.Common.Dtos;
using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Application.Movies.Services;
using System.Security.Claims;

namespace MoviesDb.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("{movieIdOrSlug}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieResponse>> GetMovieByIdOrSlugAsync(string movieIdOrSlug)
        {
            var movie = await _movieService.GetByIdOrSlugAsync(movieIdOrSlug);
            return Ok(movie);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<PagedResponse<MovieResponse>> GetMoviesAsync([FromQuery] GetMoviesListRequest request)
        {
            return _movieService.GetMoviesAsync(request);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MovieResponse>> CreateMovieAsync([FromBody] CreateMovieRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var movie = await _movieService.CreateMovieAsync(request, userId);
            return CreatedAtAction("GetMovieByIdOrSlug", new { movieIdOrSlug = movie.Id.ToString() }, movie);
        }

        [HttpPut("{movieId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MovieResponse>> UpdateMovieAsync(Guid movieId, [FromBody] UpdateMovieRequest request)
        {
            var movie = await _movieService.UpdateMovieAsync(movieId, request);
            return Ok(movie);
        }

        [HttpDelete("{movieId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteMovieAsync(Guid movieId)
        {
            await _movieService.DeleteMovieAsync(movieId);
            return NoContent();
        }
    }
}
