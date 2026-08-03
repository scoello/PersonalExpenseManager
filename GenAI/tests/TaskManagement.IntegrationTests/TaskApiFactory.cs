using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Api.Data;

namespace TaskManagement.IntegrationTests;

public sealed class TaskApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.Single(x => x.ServiceType == typeof(DbContextOptions<TaskDbContext>));
            services.Remove(descriptor);
            services.AddDbContext<TaskDbContext>(options => options.UseInMemoryDatabase("integration-" + Guid.NewGuid()));
        });
    }
}
