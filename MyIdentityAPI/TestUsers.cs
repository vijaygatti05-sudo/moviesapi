
using Duende.IdentityServer.Test;

namespace MyIdentityAPI;

public static class TestUsers
{
    public static List<TestUser> Users =>
    [
        new TestUser
        {
            SubjectId = "1",

            Username = "admin",

            Password = "password",

            Claims =
            [
                new("name", "Movie Administrator"),
                new("email", "admin@movies.com")
            ]
        },

        new TestUser
        {
            SubjectId = "2",

            Username = "user",

            Password = "password",

            Claims =
            [
                new("name", "Movie User")
            ]
        }
    ];
}