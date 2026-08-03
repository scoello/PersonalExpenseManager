using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PersonalExpenses.Infrastructure;
namespace PersonalExpenses.IntegrationTests.Fixtures;
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName=$"expenses-{Guid.NewGuid():N}";
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_,configuration)=>configuration.AddInMemoryCollection(new Dictionary<string,string?>{{"Database:InitializeOnStartup","true"}}));
        builder.ConfigureServices(services=>{services.RemoveAll<DbContextOptions>();services.RemoveAll<DbContextOptions<ExpensesDbContext>>();services.RemoveAll<Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration<ExpensesDbContext>>();services.RemoveAll<ExpensesDbContext>();services.AddDbContext<ExpensesDbContext>(options=>options.UseInMemoryDatabase(databaseName));});
    }
}
