using MoviesDb.Application.Movies.Services;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Domain.Models;
using FluentValidation;
using MoviesDb.Application.Movies.Dtos;
using MoviesDb.Application.Common.Dtos;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace MoviesDb.Tests
{
    public class MovieServiceTests
    {
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IValidator<Movie>> _validatorMock;
        private readonly TimeProvider _timeProviderMock;
        private readonly MovieService _movieService;

        public MovieServiceTests()
        {
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _validatorMock = new Mock<IValidator<Movie>>();
            _timeProviderMock = new FakeTimeProvider(DateTimeOffset.UtcNow.AddHours(-23));
            _movieService = new MovieService(_movieRepositoryMock.Object, _validatorMock.Object, _timeProviderMock);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMovie_WhenMovieExists()
        {
            // Arrange
            var movieId = Guid.NewGuid();
            var movie = new Movie
            {
                Id = movieId,
                Title = "Test Movie",
                YearOfRelease = 2020
            };
            _movieRepositoryMock.Setup(x => x.GetMovieByIdAsync(movieId)).ReturnsAsync(movie);

            // Act
            var result = await _movieService.GetByIdAsync(movieId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(movieId);
            result.Title.Should().Be(movie.Title);
            result.YearOfRelease.Should().Be(movie.YearOfRelease);
        }

        [Fact]
        public async Task GetBySlugAsync_ShouldReturnMovie_WhenMovieExists()
        {
            // Arrange
            var slug = "test-movie-2020";
            var movie = new Movie
            {
                Id = Guid.NewGuid(),
                Title = "Test Movie",
                YearOfRelease = 2020
            };
            _movieRepositoryMock.Setup(x => x.GetMovieBySlugAsync(slug)).ReturnsAsync(movie);

            // Act
            var result = await _movieService.GetBySlugAsync(slug);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(movie.Id);
            result.Title.Should().Be(movie.Title);
            result.YearOfRelease.Should().Be(movie.YearOfRelease);
        }

        [Fact]
        public async Task GetByIdOrSlugAsync_ShouldReturnMovie_WhenMovieExists()
        {
            // Arrange
            var movieId = Guid.NewGuid().ToString();
            var movie = new Movie
            {
                Id = Guid.NewGuid(),
                Title = "Test Movie",
                YearOfRelease = 2020
            };
            _movieRepositoryMock.Setup(x => x.GetMovieByIdAsync(Guid.Parse(movieId))).ReturnsAsync(movie);

            // Act
            var result = await _movieService.GetByIdOrSlugAsync(movieId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(movie.Id);
            result.Title.Should().Be(movie.Title);
            result.YearOfRelease.Should().Be(movie.YearOfRelease);
        }

        [Fact]
        public async Task GetMoviesAsync_ShouldReturnPagedMovies_WhenMoviesExist()
        {
            // Arrange
            var request = new GetMoviesListRequest();
            var pagedResponse = new PagedResponse<Movie>
            {
                PageSize = 10,
                Page = 1,
                TotalItems = 100,
                Items = new List<Movie>()
            };
            _movieRepositoryMock.Setup(x => x.GetMoviesAsync(request)).ReturnsAsync(pagedResponse);

            // Act
            var result = await _movieService.GetMoviesAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.PageSize.Should().Be(pagedResponse.PageSize);
            result.Page.Should().Be(pagedResponse.Page);
            result.TotalItems.Should().Be(pagedResponse.TotalItems);
            result.Items.Should().BeEquivalentTo(pagedResponse.Items);
        }       

        [Fact]
        public async Task DeleteMovieAsync_ShouldReturnTrue_WhenMovieExists()
        {
            // Arrange
            var movieId = Guid.NewGuid();
            _movieRepositoryMock.Setup(x => x.DeleteMovieAsync(movieId)).ReturnsAsync(1);

            // Act
            var result = await _movieService.DeleteMovieAsync(movieId);

            // Assert
            result.Should().BeTrue();
        }
    }
}
