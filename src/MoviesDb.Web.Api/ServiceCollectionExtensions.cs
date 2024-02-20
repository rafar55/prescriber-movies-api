using FluentValidation;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Movies.Services;
using MoviesDb.Application.Movies.Validators;
using MoviesDb.Infrastructure.Database;
using MoviesDb.Infrastructure.Repositories;

namespace MoviesDb.Web.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<MovieValidator>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory>(x => new SqlServerConnectionFactory(configuration.GetConnectionString("DefaultConnection")!));
        services.AddScoped<IMovieRepository, MovieRepository>();
        return services;
    }
}
