using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PersonalExpenses.Application;
using PersonalExpenses.IntegrationTests.Fixtures;
namespace PersonalExpenses.IntegrationTests.Api;
public sealed class UsersApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    public UsersApiTests(ApiFactory factory)=>client=factory.CreateClient();
    [Fact] public async Task Administrator_can_create_and_list_user()
    {
        await AuthenticateAsync();var username=$"user-{Guid.NewGuid():N}";var created=await client.PostAsJsonAsync("/api/users",new CreateUserRequest(username,"Password1!"));Assert.Equal(HttpStatusCode.Created,created.StatusCode);var users=await client.GetFromJsonAsync<List<UserDto>>("/api/users");Assert.Contains(users!,x=>x.Username==username);
    }
    [Fact] public async Task Duplicate_username_returns_conflict()
    {
        await AuthenticateAsync();var username=$"user-{Guid.NewGuid():N}";await client.PostAsJsonAsync("/api/users",new CreateUserRequest(username,"Password1!"));Assert.Equal(HttpStatusCode.Conflict,(await client.PostAsJsonAsync("/api/users",new CreateUserRequest(username,"Password1!"))).StatusCode);
    }
    [Fact] public async Task Invalid_password_returns_bad_request()
    {
        await AuthenticateAsync();Assert.Equal(HttpStatusCode.BadRequest,(await client.PostAsJsonAsync("/api/users",new CreateUserRequest($"user-{Guid.NewGuid():N}","short"))).StatusCode);
    }
    private async Task AuthenticateAsync(){var response=await client.PostAsJsonAsync("/api/auth/login",new LoginRequest("admin","Admin123!"));response.EnsureSuccessStatusCode();var session=await response.Content.ReadFromJsonAsync<LoginResponse>();client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",session!.Token);}
}
