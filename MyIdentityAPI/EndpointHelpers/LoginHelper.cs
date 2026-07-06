using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;

namespace MyIdentityAPI.EndpointHelpers;

public static class LoginHelper
{
    public static Func<IResult> RenderLoginPage()
    {
        return () =>
        {
            return Results.Content(
                """
                <html>
                <body>
                    <h2>Login</h2>

                    <form method="post">

                        <input
                            name="username"
                            placeholder="username" />

                        <br/><br/>

                        <input
                            type="password"
                            name="password"
                            placeholder="password" />

                        <br/><br/>

                        <button type="submit">
                            Login
                        </button>

                    </form>

                </body>
                </html>
                """,
                "text/html");
        };
    }
    
    public static Func<HttpContext, Task<IResult>> PostLogin()
    {
        return async (HttpContext context) =>
        {
            var form =
                await context.Request.ReadFormAsync();

            var username =
                form["username"].ToString();

            var password =
                form["password"].ToString();

            var user = TestUsers.Users.FirstOrDefault(ValidateLogin(username, password));
            if (user == null)
            {
                return Results.Unauthorized();
            }

            var claims =
                new[]
                {
                    new Claim(
                        "sub",
                        "1"),

                    new Claim(
                        "name",
                        username)
                };

            var identity =
                new ClaimsIdentity(
                    claims,
                    "cookie");

            var principal =
                new ClaimsPrincipal(
                    identity);
            
            var returnUrl =
                context.Request.Query["ReturnUrl"];
            
            await context.SignInAsync(
                new IdentityServerUser("1")
                {
                    DisplayName = username
                });
            
            return Results.Redirect(returnUrl!);
        };
    }

    private static Func<TestUser, bool> ValidateLogin(string username, string password)
    {
        return x => x.Username == username && x.Password == password;
    }


    public static Func<HttpContext, IIdentityServerInteractionService, CancellationToken, Task<IResult>> ProcessLogout()
    {
        return async (HttpContext context, IIdentityServerInteractionService interaction, CancellationToken ctx) =>
        {
            var logoutId =
                context.Request.Query["logoutId"]
                    .ToString();

            await context.SignOutAsync(
                "cookie");

            //var ctx = new CancellationToken();
            var logoutContext =
                await interaction.GetLogoutContextAsync(logoutId, ctx);

            if (!string.IsNullOrEmpty(
                    logoutContext?.PostLogoutRedirectUri))
            {
                return Results.Redirect(
                    logoutContext.PostLogoutRedirectUri);
            }

            return Results.Content(
                """
                <h2>
                    Logged out successfully
                </h2>
                """,
                "text/html");

            return Results.Ok();
        };
    }

}