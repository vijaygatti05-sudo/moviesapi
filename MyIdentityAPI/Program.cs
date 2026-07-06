using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Microsoft.IdentityModel.Tokens;
using MyIdentityAPI;
using MyIdentityAPI.EndpointHelpers;
using System.Security.Claims;
using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
//     .AddIdentityServer(options =>
//     {
//         options.EmitStaticAudienceClaim = true;
//     })
//     .AddInMemoryIdentityResources(Config.IdentityResources)
//     .AddInMemoryApiScopes(Config.ApiScopes)
//     .AddInMemoryApiResources(Config.ApiResources)
//     .AddInMemoryClients(Config.Clients)
//     .AddTestUsers(TestUsers.Users);

builder.Services
    .AddAuthentication()
    .AddCookie("cookie",
        options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
        });

builder.Services
    .AddIdentityServer(options =>
    {
        options.Authentication.CookieAuthenticationScheme = "cookie";
        options.EmitStaticAudienceClaim = true;
        
    })
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryApiResources(Config.ApiResources);

var app = builder.Build();

app.UseCors("React");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseIdentityServer();

app.MapGet("/login", LoginHelper.RenderLoginPage());

app.MapPost("/login", LoginHelper.PostLogin());

app.MapGet("/logout", LoginHelper.ProcessLogout());

app.Run();

// app.MapPost("/login",
//     (LoginRequest request,
//         IConfiguration config) =>
//     {
//         // fake user validation
//         if (request.Username != "admin" ||
//             request.Password != "password")
//         {
//             return Results.Unauthorized();
//         }
//
//         var token =
//             JwtGenerator.Generate(
//                 config,
//                 request.Username);
//
//         return Results.Ok(new
//         {
//             access_token = token
//         });
//     });
//
// app.MapPost("/token", (LoginRequest request) =>
// {
//     if (request.Username != "admin" ||
//         request.Password != "password")
//     {
//         return Results.Unauthorized();
//     }
//
//     var token = CreateJwtToken();
//
//     string CreateJwtToken()
//     {
//         
//         const string SECRET_KEY =
//             "THIS_IS_MY_SUPER_SECRET_KEY_FOR_DEMO_ONLY_12345";
//
//         var claims = new[]
//         {
//             new Claim(JwtRegisteredClaimNames.Sub, "1"),
//             new Claim(JwtRegisteredClaimNames.Email, "admin@test.com"),
//             new Claim(ClaimTypes.Name, "admin"),
//             new Claim(ClaimTypes.Role, "Admin"),
//             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//         };
//
//         var key = new SymmetricSecurityKey(
//             Encoding.UTF8.GetBytes(SECRET_KEY));
//
//         var credentials = new SigningCredentials(
//             key,
//             SecurityAlgorithms.HmacSha256);
//
//         var token = new JwtSecurityToken(
//             issuer: "my-identity-server",
//             audience: "movies-api",
//             claims: claims,
//             expires: DateTime.UtcNow.AddHours(1),
//             signingCredentials: credentials);
//
//         return new JwtSecurityTokenHandler()
//             .WriteToken(token);
//     }
//     
//     return Results.Ok(new
//     {
//         access_token = token
//     });
// });
//
// app.MapGet("/weatherforecast", () =>
//     {
//         return "request successfull";
//     })
//     .WithName("GetWeatherForecast");
//
// app.Run();
