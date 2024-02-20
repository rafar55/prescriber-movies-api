using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesDb.Infrastructure.Authentication;
using MoviesDb.Infrastructure.Database;
using MoviesDb.Web.Api;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MoviesDb.Web.Api", Version = "v1" });

    // Define the BearerAuth scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Reference the BearerAuth scheme in the operation's security requirements
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       var jwtConfig = builder.Configuration.GetSection(JwtTokenConfig.SectionName).Get<JwtTokenConfig>()!;
       options.TokenValidationParameters = new TokenValidationParameters()
       {
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidIssuer = jwtConfig.Issuer,
           ValidAudience = jwtConfig.Audience,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true
       };
   });
builder.Services.AddAuthorization();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseProblemDetails();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

//This is not for production
// but fot the purpose of the challenge it will work
await RunDbInitializer(app);

app.Run();


async Task RunDbInitializer(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await dbInitializer.RunMigrationAsync();
    await dbInitializer.SeedDatabaseAsync();
}