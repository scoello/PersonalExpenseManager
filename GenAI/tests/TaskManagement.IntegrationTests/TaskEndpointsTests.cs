using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagement.Api.Contracts;

namespace TaskManagement.IntegrationTests;

public sealed class TaskEndpointsTests : IClassFixture<TaskApiFactory>
{
    private readonly HttpClient _client;
    private readonly Guid _user = Guid.NewGuid();
    public TaskEndpointsTests(TaskApiFactory factory) { _client = factory.CreateClient(); _client.DefaultRequestHeaders.Add("X-User-Id", _user.ToString()); }

    [Fact]
    public async Task Crud_flow_succeeds()
    {
        var create = await _client.PostAsJsonAsync("/api/v1/tasks", new { title = "Write tests", description = "All cases", status = "pending" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var task = await create.Content.ReadFromJsonAsync<TaskResponse>(); Assert.NotNull(task);
        var get = await _client.GetAsync($"/api/v1/tasks/{task.Id}"); Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var patch = await _client.PatchAsJsonAsync($"/api/v1/tasks/{task.Id}", new { status = "completed", due_date = (DateTimeOffset?)null });
        Assert.Equal(HttpStatusCode.OK, patch.StatusCode);
        var delete = await _client.DeleteAsync($"/api/v1/tasks/{task.Id}"); Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/v1/tasks/{task.Id}")).StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")]
    public async Task Create_rejects_invalid_title(string title)
    { var response = await _client.PostAsJsonAsync("/api/v1/tasks", new { title }); Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); }

    [Fact]
    public async Task Missing_and_foreign_tasks_are_indistinguishable()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/v1/tasks/{Guid.NewGuid()}")).StatusCode);
        var created = await (await _client.PostAsJsonAsync("/api/v1/tasks", new { title = "Secret" })).Content.ReadFromJsonAsync<TaskResponse>();
        using var other = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/tasks/{created!.Id}"); other.Headers.Add("X-User-Id", Guid.NewGuid().ToString());
        Assert.Equal(HttpStatusCode.NotFound, (await _client.SendAsync(other)).StatusCode);
    }

    [Fact]
    public async Task List_supports_filters_and_pagination()
    {
        await _client.PostAsJsonAsync("/api/v1/tasks", new { title = "One", status = "pending", due_date = "2026-08-10T12:00:00Z" });
        await _client.PostAsJsonAsync("/api/v1/tasks", new { title = "Two", status = "completed", due_date = "2026-08-11T12:00:00Z" });
        var response = await _client.GetAsync("/api/v1/tasks?status=pending&dueFrom=2026-08-01T00:00:00Z&dueTo=2026-08-31T00:00:00Z&page=1&pageSize=1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1, json.RootElement.GetProperty("total_count").GetInt32()); Assert.Single(json.RootElement.GetProperty("items").EnumerateArray());
    }

    [Fact]
    public async Task Pagination_validation_returns_problem_details()
    { var response = await _client.GetAsync("/api/v1/tasks?page=0&pageSize=101"); Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType); }
}
