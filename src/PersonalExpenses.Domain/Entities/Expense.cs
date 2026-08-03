namespace PersonalExpenses.Domain.Entities;

public sealed class Expense
{
    private Expense() { }

    public Expense(Guid userId, DateOnly date, decimal amount, string category)
    {
        if (userId == Guid.Empty) throw new ArgumentException("A user is required.");
        Id = Guid.NewGuid();
        UserId = userId;
        Update(date, amount, category);
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateOnly Date { get; private set; }
    public decimal Amount { get; private set; }
    public string Category { get; private set; } = string.Empty;

    public void Update(DateOnly date, decimal amount, string category)
    {
        if (amount <= 0) 
            throw new ArgumentOutOfRangeException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(category)) 
            throw new ArgumentException("Category is required.");

        Date = date;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Category = category.Trim();
    }
}
