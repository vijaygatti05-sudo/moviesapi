using System.Text;
using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MoviesWEBAPI;

//using System.Web.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

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

// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options => { MoviesWEBAPI.Auth.Config.ConfigureJWTBearerOptions(options); });
//
// builder.Services.AddAuthorization(options => { MoviesWEBAPI.Auth.Config.ConfigureAUthorizeOptions(options); });

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1, 0))
    .HasApiVersion(new ApiVersion(2, 0))
    .ReportApiVersions()
    .Build();

app.UseMiddleware<LogRequestMiddleware>();

app.UseCors("React");

app.UseMiddleware<FirstCustomMiddleware>();

app.UseHttpsRedirection();

app.UseMiddleware<SecondCustomMiddleware>();

// app.UseAuthentication();
//
// app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/v{version:apiVersion}/movies", 
        //[Authorize(Policy = "movies")] 
        () =>
    {
        var path = "Data/movies.json";
        var json = File.ReadAllText(path);

            
        Thread.Sleep(5000);
        var movies = JsonSerializer.Deserialize<List<Movie>>(json)
                     ?? new List<Movie>();

        return Results.Ok(movies);
        
        // return Results.Ok(new
        // {
        //     movies = movies,
        //     version = "1.0"
        // });
    })
    .WithName("GetMoviesListV1")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(1.0);



app.MapGet("/v{version:apiVersion}/movies", 
        //[Authorize(Policy = "movies")] 
        () =>
        {
            var path = "Data/movies.json";
            var json = File.ReadAllText(path);

            
            Thread.Sleep(5000);
            var movies = JsonSerializer.Deserialize<List<Movie>>(json)
                         ?? new List<Movie>();

            return Results.Ok(movies);
            
            // return Results.Ok(new
            // {
            //     movies = movies,
            //     version = "2.0"
            // });

        })
    .WithName("GetMoviesListV2")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(2.0);
    

app.Run();

record Movie(string id, string title, string subtitle, string description, string imageUrl, int[] ratings)
{
}

