using PersonalExpenses.IntegrationTests.Fixtures;
namespace PersonalExpenses.IntegrationTests.Fixtures;
[CollectionDefinition(Name)]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerDatabaseFixture>
{
    public const string Name = "SQL Server repository tests";
}
