namespace PersonalExpenses.Application;
public sealed record ExpenseDto(Guid Id, DateOnly Date, decimal Amount, string Category);
