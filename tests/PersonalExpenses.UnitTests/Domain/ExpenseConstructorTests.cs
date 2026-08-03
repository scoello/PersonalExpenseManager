using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.UnitTests.Domain;
public sealed class ExpenseConstructorTests
{
    [Fact]
    public void Valid_values_create_expense_and_normalize_fields()
    { 
        //Arrange
        var userId=Guid.NewGuid();

        //Act
        var expense = new Expense(userId, new DateOnly(2026, 7, 31), 12.345m, " Food ");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.NotEqual(Guid.Empty, expense.Id);
            Assert.Equal(userId, expense.UserId);
            Assert.Equal(new DateOnly(2026, 7, 31), expense.Date);
            Assert.Equal(12.35m, expense.Amount);
            Assert.Equal("Food", expense.Category);
        });
    }

    [Fact]
    public void Empty_user_id_is_rejected()
    {
        //Arrange

        //Act
        var exception = Assert.Throws<ArgumentException>(() => new Expense(Guid.Empty, DateOnly.FromDateTime(DateTime.Today), 10m, "Food"));

        //Assert
        Assert.Equal("A user is required.", exception.Message);

    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Non_positive_amount_is_rejected(decimal amount)
    {
        //Arrange

        //Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Expense(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), amount, "Food"));

        //Assert
        Assert.Equal("Specified argument was out of the range of valid values. (Parameter 'Amount must be greater than zero.')", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_category_is_rejected(string category)
    {
        //Arrange

        //Act
        var exception = Assert.Throws<ArgumentException>(() => new Expense(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), 10m, category));

        //Assert
        Assert.Equal("Category is required.", exception.Message);
    }
}
