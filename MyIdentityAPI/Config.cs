using Duende.IdentityServer.Models;

namespace MyIdentityAPI;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope(
            name: "moviesapi.read",
            displayName: "Movies API")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource(
            name: "moviesapi",
            displayName: "Movies API")
        {
            Scopes =
            {
                "moviesapi.read"
            }
        }
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "movies-react",
            ClientName = "Movies React SPA",
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RequireClientSecret = false,
            RedirectUris =
            {
                "http://localhost:3000/callback"
            },
            PostLogoutRedirectUris =
            { 
                "http://localhost:3000/logout-callback"
            },
            AllowedCorsOrigins =
            {
                "http://localhost:3000"
            },

            AllowedScopes =
            {
                "openid",
                "profile",
                "moviesapi.read"
            },

            AllowOfflineAccess = true
        }
    ];
}