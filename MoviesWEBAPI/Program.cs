using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("React", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

const string SECRET_KEY =
    "THIS_IS_MY_SUPER_SECRET_KEY_FOR_DEMO_ONLY_12345";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // options.TokenValidationParameters =
        //     new TokenValidationParameters
        //     {
        //         ValidateIssuer = false,
        //         ValidateAudience = false,
        //         ValidateLifetime = true,
        //         ValidateIssuerSigningKey = true,
        //         IssuerSigningKey =
        //             new SymmetricSecurityKey(
        //                 Encoding.UTF8.GetBytes(SECRET_KEY))
        //     };
        
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "my-identity-server",

                ValidateAudience = true,
                ValidAudience = "movies-api",

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(SECRET_KEY))
            };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("React");

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// app.MapPost("/token", () =>
// {
//     string SECRET_KEY =
//         "THIS_IS_MY_SUPER_SECRET_KEY_FOR_DEMO_ONLY_12345";
//
//     
//     var claims = new[]
//     {
//         new Claim(ClaimTypes.Name, "react-user")
//     };
//
//     var key =
//         new SymmetricSecurityKey(
//             Encoding.UTF8.GetBytes(SECRET_KEY));
//
//     var credentials =
//         new SigningCredentials(
//             key,
//             SecurityAlgorithms.HmacSha256);
//
//     var token = new JwtSecurityToken(
//         expires: DateTime.UtcNow.AddHours(1),
//         claims: claims,
//         signingCredentials: credentials);
//
//     return Results.Ok(new
//     {
//         token = new JwtSecurityTokenHandler().WriteToken(token)
//     });
// });

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

// app.MapGet("/movies", () =>
//     {
//         var movies = ""; // get jasonlist from movies.json file 
//         return movies;
//     })
//     .WithName("GetMoviesList");

app.MapGet("/movies", [Microsoft.AspNetCore.Authorization.Authorize] () =>
    {
        var path = "Data/movies.json";
        var json = File.ReadAllText(path);

            
        Thread.Sleep(5000);
        var movies = JsonSerializer.Deserialize<List<Movie>>(json)
                     ?? new List<Movie>();

        return Results.Ok(movies);
    })
    .WithName("GetMoviesList");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record Movie(string id, string title, string subtitle, string description, string imageUrl, int[] ratings)
{
}

