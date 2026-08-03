using PersonalExpenses.Domain.Entities;
namespace PersonalExpenses.UnitTests.Domain;
public sealed class ExpenseUpdateTests
{
    private static Expense Create() => new(Guid.NewGuid(), new DateOnly(2026, 7, 31), 10m, "Food");

    [Fact]
    public void Valid_values_update_and_normalize_expense()
    { 
        //Arrange
        var expense = Create();

        //Act
        expense.Update(new DateOnly(2026,8,1),99.995m," Travel ");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(new DateOnly(2026, 8, 1), expense.Date);
            Assert.Equal(100m, expense.Amount);
            Assert.Equal("Travel", expense.Category);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Non_positive_amount_is_rejected(decimal amount)
    { 
        //Arrange
        var expense = Create();

        //Act
        Assert.Throws<ArgumentOutOfRangeException>(() => expense.Update(new DateOnly(2026,8,1),amount,"Travel"));

        //Assert
        Assert.Equal(10m, expense.Amount);
    }

    [Fact]
    public void Blank_category_is_rejected_without_mutating_expense() 
    { 
        //Arrange
        var expense = Create();

        //Act
        Assert.Throws<ArgumentException>(()=>expense.Update(new DateOnly(2026,8,1),20m," "));

        //Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(new DateOnly(2026, 7, 31), expense.Date);
            Assert.Equal(10m, expense.Amount);
            Assert.Equal("Food", expense.Category);
        });
    }

}
