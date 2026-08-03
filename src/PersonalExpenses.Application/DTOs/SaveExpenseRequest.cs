namespace PersonalExpenses.Application;
public sealed record SaveExpenseRequest(DateOnly Date, decimal Amount, string Category);
