using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure;
using PersonalExpenses.IntegrationTests.Fixtures;
namespace PersonalExpenses.IntegrationTests.Database;
[Collection(SqlServerCollection.Name)]
public sealed class ExpenseRepositoryTests(SqlServerDatabaseFixture database)
{
    [Fact] public async Task Add_and_list_persist_expense_and_filter_by_user()
    {
        var owner=await CreateUserAsync("owner"); var other=await CreateUserAsync("other");
        await using(var context=database.CreateContext()){var repository=new ExpenseRepository(context);await repository.AddAsync(new Expense(owner.Id,new DateOnly(2026,7,30),10m,"Food"),default);await repository.AddAsync(new Expense(owner.Id,new DateOnly(2026,7,31),20m,"Travel"),default);await repository.AddAsync(new Expense(other.Id,new DateOnly(2026,8,1),30m,"Other"),default);await repository.SaveChangesAsync(default);}
        await using var verification=database.CreateContext(); var results=await new ExpenseRepository(verification).ListAsync(owner.Id,default); Assert.Equal(2,results.Count); Assert.Equal("Travel",results[0].Category); Assert.DoesNotContain(results,x=>x.UserId==other.Id);
    }
    [Fact] public async Task Get_does_not_return_another_users_expense()
    {
        var owner=await CreateUserAsync($"owner-{Guid.NewGuid():N}"); var expense=new Expense(owner.Id,new DateOnly(2026,7,31),10m,"Food"); await using(var context=database.CreateContext()){context.Expenses.Add(expense);await context.SaveChangesAsync();}
        await using var verification=database.CreateContext(); var repository=new ExpenseRepository(verification); Assert.Null(await repository.GetAsync(expense.Id,Guid.NewGuid(),default)); Assert.NotNull(await repository.GetAsync(expense.Id,owner.Id,default));
    }
    [Fact] public async Task Save_changes_persists_updated_expense()
    {
        var owner=await CreateUserAsync($"owner-{Guid.NewGuid():N}"); var expense=new Expense(owner.Id,new DateOnly(2026,7,31),10m,"Food"); await using(var context=database.CreateContext()){context.Expenses.Add(expense);await context.SaveChangesAsync();}
        await using(var context=database.CreateContext()){var repository=new ExpenseRepository(context);var stored=await repository.GetAsync(expense.Id,owner.Id,default);stored!.Update(new DateOnly(2026,8,1),25m,"Travel");await repository.SaveChangesAsync(default);}
        await using var verification=database.CreateContext(); var persisted=await verification.Expenses.AsNoTracking().SingleAsync(x=>x.Id==expense.Id); Assert.Equal(25m,persisted.Amount); Assert.Equal("Travel",persisted.Category);
    }
    [Fact] public async Task Remove_deletes_expense_from_database()
    {
        var owner=await CreateUserAsync($"owner-{Guid.NewGuid():N}"); var expense=new Expense(owner.Id,new DateOnly(2026,7,31),10m,"Food"); await using(var context=database.CreateContext()){context.Expenses.Add(expense);await context.SaveChangesAsync();}
        await using(var context=database.CreateContext()){var repository=new ExpenseRepository(context);repository.Remove((await repository.GetAsync(expense.Id,owner.Id,default))!);await repository.SaveChangesAsync(default);}
        await using var verification=database.CreateContext(); Assert.False(await verification.Expenses.AnyAsync(x=>x.Id==expense.Id));
    }
    private async Task<AppUser> CreateUserAsync(string username){var user=new AppUser(username,"hash");await using var context=database.CreateContext();context.Users.Add(user);await context.SaveChangesAsync();return user;}
}
