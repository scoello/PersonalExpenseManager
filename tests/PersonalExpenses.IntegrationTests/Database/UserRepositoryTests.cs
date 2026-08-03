using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure;
using PersonalExpenses.IntegrationTests.Fixtures;
namespace PersonalExpenses.IntegrationTests.Database;
[Collection(SqlServerCollection.Name)]
public sealed class UserRepositoryTests(SqlServerDatabaseFixture database)
{
    [Fact] public async Task Add_and_find_persist_user_with_normalized_username()
    {
        var username=$"User-{Guid.NewGuid():N}"; var user=new AppUser($" {username} ","hash");
        await using(var context=database.CreateContext()){var repository=new UserRepository(context);await repository.AddAsync(user,default);await repository.SaveChangesAsync(default);}
        await using var verification=database.CreateContext(); var persisted=await new UserRepository(verification).FindAsync(username.ToUpperInvariant(),default); Assert.NotNull(persisted); Assert.Equal(username.ToLowerInvariant(),persisted.Username); Assert.Equal("hash",persisted.PasswordHash);
    }
    [Fact] public async Task Find_returns_null_for_unknown_username()
    {
        await using var context=database.CreateContext(); Assert.Null(await new UserRepository(context).FindAsync($"missing-{Guid.NewGuid():N}",default));
    }
    [Fact] public async Task List_returns_users_ordered_by_username()
    {
        var suffix=Guid.NewGuid().ToString("N"); await using(var context=database.CreateContext()){var repository=new UserRepository(context);await repository.AddAsync(new AppUser($"z-{suffix}","hash"),default);await repository.AddAsync(new AppUser($"a-{suffix}","hash"),default);await repository.SaveChangesAsync(default);}
        await using var verification=database.CreateContext(); var results=await new UserRepository(verification).ListAsync(default); Assert.Equal(results.OrderBy(x=>x.Username).Select(x=>x.Id),results.Select(x=>x.Id)); Assert.Contains(results,x=>x.Username==$"a-{suffix}"); Assert.Contains(results,x=>x.Username==$"z-{suffix}");
    }
}
