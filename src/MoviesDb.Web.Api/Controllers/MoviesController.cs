using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesDb.Application.Common.Dtos;
using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Application.Movies.Services;

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovieResponse>> CreateMovieAsync([FromBody] CreateMovieRequest request)
        {
            var movie = await _movieService.CreateMovieAsync(request, Guid.NewGuid());
            return CreatedAtAction(nameof(GetMovieByIdOrSlugAsync), new { movieIdOrSlug = movie.Id }, movie);
        }

        [HttpPut("{movieId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieResponse>> UpdateMovieAsync(Guid movieId, [FromBody] UpdateMovieRequest request)
        {
            var movie = await _movieService.UpdateMovieAsync(movieId, request);
            return Ok(movie);
        }

        [HttpDelete("{movieId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMovieAsync(Guid movieId)
        {
            await _movieService.DeleteMovieAsync(movieId);
            return NoContent();
        }
    }
}
