using PersonalExpenses.Application;
using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.UnitTests.TestDoubles;

internal sealed class FakeExpenseRepository : IExpenseRepository
{
    private readonly List<Expense> expenses = [];
    public int SaveCount { get; private set; }
    public void Seed(Expense expense) => expenses.Add(expense);

    public Task<IReadOnlyList<Expense>> ListAsync(Guid userId, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Expense>>(expenses.Where(x => x.UserId == userId).ToList());
    }

    public Task<Expense?> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(expenses.SingleOrDefault(x => x.Id == id && x.UserId == userId));
    }

    public Task AddAsync(Expense expense, CancellationToken cancellationToken) 
    { 
        expenses.Add(expense); return Task.CompletedTask; 
    }

    public void Remove(Expense expense) => expenses.Remove(expense);
    public Task SaveChangesAsync(CancellationToken cancellationToken) 
    { 
        SaveCount++; 
        return Task.CompletedTask; 
    }
}
