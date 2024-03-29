﻿using FluentMigrator.Runner;
using FluentValidation;
using Microsoft.Extensions.Options;
using MoviesDb.Application.Common.Interfaces;
using MoviesDb.Application.Movies.Services;
using MoviesDb.Application.Movies.Validators;
using MoviesDb.Application.Users.Services;
using MoviesDb.Infrastructure.Authentication;
using MoviesDb.Infrastructure.Database;
using MoviesDb.Infrastructure.Repositories;
using MoviesDb.Web.Api.Extensions;
using Hellang.Middleware.ProblemDetails;


namespace MoviesDb.Web.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddSingleton(x => TimeProvider.System);
        services.AddValidatorsFromAssemblyContaining<MovieValidator>();
       

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var masterConnection = configuration.GetConnectionString("MasterConnection")!;
        services.AddSingleton<IDbConnectionFactory>(x => new SqlServerConnectionFactory(connectionString, masterConnection));
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenProvider, JwtTokenProvider>();

        services.Configure<JwtTokenConfig>(configuration.GetSection(JwtTokenConfig.SectionName));
        services.AddSingleton(ctx => ctx.GetRequiredService<IOptions<JwtTokenConfig>>().Value);


        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(DbInitializer).Assembly).For.Migrations());
        services.AddScoped<DbInitializer>();

        services.AddProblemDetails(options =>
        {
            options.IncludeExceptionDetails = (_,_) => false;
            options.MapFluentValidation();
            options.MapApplicationExceptions();
        });

        return services;
    }
}
