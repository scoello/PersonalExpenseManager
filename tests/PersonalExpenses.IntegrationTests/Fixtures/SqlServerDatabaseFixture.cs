using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Infrastructure;
namespace PersonalExpenses.IntegrationTests.Fixtures;
public sealed class SqlServerDatabaseFixture : IAsyncLifetime
{
    private readonly string masterConnectionString;
    public SqlServerDatabaseFixture()
    {
        var configured = Environment.GetEnvironmentVariable("PERSONAL_EXPENSES_TEST_SQLSERVER") ?? "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True";
        var builder = new SqlConnectionStringBuilder(configured) { InitialCatalog = "master" };
        masterConnectionString = builder.ConnectionString;
        builder.InitialCatalog = DatabaseName;
        ConnectionString = builder.ConnectionString;
    }
    public string DatabaseName { get; } = $"PersonalExpensesTests_{Guid.NewGuid():N}";
    public string ConnectionString { get; }
    public ExpensesDbContext CreateContext() => new(new DbContextOptionsBuilder<ExpensesDbContext>().UseSqlServer(ConnectionString).Options);
    public async Task InitializeAsync()
    {
        await using var master = new SqlConnection(masterConnectionString); await master.OpenAsync();
        await using var create = new SqlCommand($"CREATE DATABASE [{DatabaseName}]", master); await create.ExecuteNonQueryAsync();
        await using var context = CreateContext(); await context.Database.EnsureCreatedAsync();
    }
    public async Task DisposeAsync()
    {
        SqlConnection.ClearAllPools();
        await using var master = new SqlConnection(masterConnectionString); await master.OpenAsync();
        await using var drop = new SqlCommand($"IF DB_ID(N'{DatabaseName}') IS NOT NULL BEGIN ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DatabaseName}]; END", master); await drop.ExecuteNonQueryAsync();
    }
}
