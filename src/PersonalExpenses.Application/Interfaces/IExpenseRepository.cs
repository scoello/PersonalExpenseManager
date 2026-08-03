using PersonalExpenses.Domain.Entities;
namespace PersonalExpenses.Application;
public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<Expense?> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Expense expense, CancellationToken cancellationToken);
    void Remove(Expense expense);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
