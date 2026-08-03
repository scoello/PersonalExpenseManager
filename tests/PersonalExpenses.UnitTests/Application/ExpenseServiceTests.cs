using PersonalExpenses.Application;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.UnitTests.TestDoubles;

namespace PersonalExpenses.UnitTests.Application;
public sealed class ExpenseServiceTests
{
    private readonly FakeExpenseRepository fakeRepository = new();
    private ExpenseService CreateService() => new(fakeRepository);

    [Fact] 
    public async Task Create_adds_and_saves_expense() 
    {
        //Arrange
        var userId=Guid.NewGuid();

        //Act
        var result = await CreateService().CreateAsync(userId,new SaveExpenseRequest(new DateOnly(2026,7,31),15m,"Food"),default);

        //Assert
        var resultList = await fakeRepository.ListAsync(userId, default);
        Assert.Multiple(() =>
        {
            Assert.Equal("Food", result.Category);
            Assert.Single(resultList);
            Assert.Equal(1, fakeRepository.SaveCount);
        });
    }

    [Fact]
    public async Task List_returns_only_requested_users_expenses() 
    {
        //Arrange
        var owner = Guid.NewGuid();
        fakeRepository.Seed(new Expense(owner,new DateOnly(2026,7,31),10m,"Food"));
        fakeRepository.Seed(new Expense(Guid.NewGuid(),new DateOnly(2026,7,31),20m,"Travel"));

        //Act
        var result = await CreateService().ListAsync(owner,default); Assert.Single(result);

        //Assert
        Assert.Equal("Food", result[0].Category);
    }

    [Fact]
    public async Task Get_returns_null_for_another_users_expense() 
    {
        //Arrange
        var expense = new Expense(Guid.NewGuid(),new DateOnly(2026,7,31),10m,"Food");

        //Act
        fakeRepository.Seed(expense);

        //Asert
        var expenseCreated = await CreateService().GetAsync(expense.Id, Guid.NewGuid(), default);
        Assert.Null(expenseCreated);
    }

    [Fact]
    public async Task Update_changes_existing_expense_and_saves() 
    {
        //Arrange
        var owner=Guid.NewGuid();
        var expense = new Expense(owner, new DateOnly(2026, 7, 31), 10m, "Food");
        fakeRepository.Seed(expense);

        //Act
        var updated = await CreateService().UpdateAsync(expense.Id, owner, new SaveExpenseRequest(new DateOnly(2026, 8, 1), 20m, "Travel"), default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.True(updated);
            Assert.Equal("Travel", expense.Category);
            Assert.Equal(1, fakeRepository.SaveCount);
        });
    }

    [Fact]
    public async Task Update_missing_expense_returns_false_without_saving() 
    {
        //Arrange
        var saveExpenseRequest = new SaveExpenseRequest(DateOnly.FromDateTime(DateTime.Today), 10m, "Food");

        //Act
        var result = await CreateService().UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), saveExpenseRequest, default);

        //Assert
        Assert.Equal(0,fakeRepository.SaveCount);
        Assert.False(result);
    }

    [Fact]
    public async Task Delete_removes_existing_expense_and_saves() 
    {
        //Arrange
        var owner = Guid.NewGuid();
        var expense = new Expense(owner,new DateOnly(2026,7,31),10m,"Food");
        fakeRepository.Seed(expense);

        //Act
        var result = await CreateService().DeleteAsync(expense.Id, owner, default);

        //Assert
        var list = await fakeRepository.ListAsync(owner, default);
        Assert.Multiple(() =>
        {
            Assert.True(result);
            Assert.Empty(list);
            Assert.Equal(1, fakeRepository.SaveCount);
        });
    }

    [Fact]
    public async Task Delete_missing_expense_returns_false_without_saving() 
    {
        //Arrange

        //Act
        var result = await CreateService().DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), default);

        //Assert
        Assert.Multiple(() =>
        {
            Assert.False(result);
            Assert.Equal(0, fakeRepository.SaveCount);
        });
    }

}
