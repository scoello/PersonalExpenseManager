using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Application;
using PersonalExpenses.Domain.Entities;
namespace PersonalExpenses.Infrastructure;
public sealed class ExpenseRepository(ExpensesDbContext dbContext) : IExpenseRepository
{
    public async Task<IReadOnlyList<Expense>> ListAsync(Guid userId, CancellationToken cancellationToken)
    {
        var expenses = await dbContext.Expenses.Where(x => x.UserId == userId)
                                               .OrderByDescending(x => x.Date)
                                               .AsNoTracking()
                                               .ToListAsync(cancellationToken);
        return expenses;
    }

    public Task<Expense?> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.Expenses.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
    }

    public Task AddAsync(Expense expense, CancellationToken cancellationToken)
    {
        return dbContext.Expenses.AddAsync(expense, cancellationToken)
                                 .AsTask();
    }

    public void Remove(Expense expense)
    {
        dbContext.Expenses.Remove(expense);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
