namespace PersonalExpenses.Application;
public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseDto>> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<ExpenseDto?> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<ExpenseDto> CreateAsync(Guid userId, SaveExpenseRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, Guid userId, SaveExpenseRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
}
