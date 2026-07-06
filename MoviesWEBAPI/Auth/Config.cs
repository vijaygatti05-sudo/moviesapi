using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MoviesWEBAPI.Auth;

public class Config
{
    public static void ConfigureJWTBearerOptions(JwtBearerOptions jwtBearerOptions)
    {
        jwtBearerOptions.Authority =
            "https://localhost:7099";

        jwtBearerOptions.TokenValidationParameters
            .ValidateAudience = false;
        
        jwtBearerOptions.Events =
            new JwtBearerEvents
            {
                OnAuthenticationFailed =
                    context =>
                    {
                        Console.WriteLine(
                            context.Exception);

                        return Task.CompletedTask;
                    },
                OnTokenValidated = async context =>
                {
                    Console.WriteLine(context);
                },
                OnChallenge = async context =>
                {
                    Console.WriteLine(context);
                }
            };
    }

    public static void ConfigureAUthorizeOptions(AuthorizationOptions authorizationOptions)
    {
        authorizationOptions.AddPolicy(
            "movies",
            policy =>
            {
                // policy.RequireClaim(
                //     "scope",
                //     "moviesapi");
                // policy.RequireClaim("scope","moviesapi.read");
                
                policy.RequireAssertion(context =>
                    context.User.HasClaim("scope", "moviesapi") ||
                    context.User.HasClaim("scope", "moviesapi.read"));
            });
    }

}