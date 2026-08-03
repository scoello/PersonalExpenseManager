using PersonalExpenses.Application;
using PersonalExpenses.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PersonalExpenses.IntegrationTests.Api;
public sealed class ExpensesApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient client;
    public ExpensesApiTests(ApiFactory factory)
    {
        client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin", "Admin123!"));
        response.EnsureSuccessStatusCode();
        var session = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session!.Token);
    }

    [Fact] 
    public async Task Authenticated_user_can_complete_expense_crud()
    {
        //Arrange
        await AuthenticateAsync();
        var saveExpenseRequest = new SaveExpenseRequest(new DateOnly(2026, 7, 31), 25.50m, "Transport");

        //Act
        var createdResponse = await client.PostAsJsonAsync("/api/expenses", saveExpenseRequest);

        //Assert
        var created = await createdResponse.Content.ReadFromJsonAsync<ExpenseDto>();
        var fetched = await client.GetFromJsonAsync<ExpenseDto>($"/api/expenses/{created!.Id}");
        var listed = await client.GetFromJsonAsync<List<ExpenseDto>>("/api/expenses");

        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        Assert.Equal("Transport", fetched!.Category);
        Assert.Contains(listed!,x => x.Id == created.Id);
        Assert.Equal(HttpStatusCode.NoContent,(await client.PutAsJsonAsync($"/api/expenses/{created.Id}",new SaveExpenseRequest(new DateOnly(2026,7,30),30m,"Travel"))).StatusCode);
        Assert.Equal("Travel",(await client.GetFromJsonAsync<ExpenseDto>($"/api/expenses/{created.Id}"))!.Category);
        Assert.Equal(HttpStatusCode.NoContent,(await client.DeleteAsync($"/api/expenses/{created.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,(await client.GetAsync($"/api/expenses/{created.Id}")).StatusCode);
    }
}
