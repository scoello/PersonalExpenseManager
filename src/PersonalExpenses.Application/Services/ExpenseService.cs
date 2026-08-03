using PersonalExpenses.Domain.Entities;
namespace PersonalExpenses.Application;
public sealed class ExpenseService(IExpenseRepository expenseRepository) : IExpenseService
{
    public async Task<IReadOnlyList<ExpenseDto>> ListAsync(Guid userId, CancellationToken cancellationToken)
    {
        var list = await expenseRepository.ListAsync(userId, cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<ExpenseDto?> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var expense = await expenseRepository.GetAsync(id, userId, cancellationToken);
        if (expense is not null)
        {
            return Map(expense);
        }

        return null;
    }

    public async Task<ExpenseDto> CreateAsync(Guid userId, SaveExpenseRequest request, CancellationToken cancellationToken) 
    { 
        var expense = new Expense(userId, request.Date, request.Amount, request.Category);
        await expenseRepository.AddAsync(expense, cancellationToken);
        await expenseRepository.SaveChangesAsync(cancellationToken);
        return Map(expense);
    }

    public async Task<bool> UpdateAsync(Guid id, Guid userId, SaveExpenseRequest request, CancellationToken cancellationToken) 
    { 
        var expense = await expenseRepository.GetAsync(id, userId, cancellationToken);
        if (expense is null) 
            return false;

        expense.Update(request.Date, request.Amount, request.Category);
        await expenseRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken) 
    { 
        var expense = await expenseRepository.GetAsync(id, userId, cancellationToken);
        if (expense is null) 
            return false; 
        
        expenseRepository.Remove(expense); 
        await expenseRepository.SaveChangesAsync(cancellationToken); 
        return true; 
    }
    
    private static ExpenseDto Map(Expense expense)
    {
        return new(expense.Id, expense.Date, expense.Amount, expense.Category);
    }
}
