using PersonalExpenses.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace PersonalExpenses.IntegrationTests.Api;
public sealed class AuthenticationApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    public AuthenticationApiTests(ApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Expenses_require_authentication()
    {
        //Arrange

        //Act
        var statusCode = (await client.GetAsync("/api/expenses")).StatusCode;

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, statusCode);
    }

    [Fact]
    public async Task Invalid_credentials_are_rejected()
    {
        //Arrange

        //Act
        var statusCode = (await client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "wrong" })).StatusCode;

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, statusCode);
    }
}

