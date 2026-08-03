using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Contracts;
using TaskManagement.Api.Data;
using TaskManagement.Api.Domain;
using TaskManagement.Api.Services;
using TaskStatus = TaskManagement.Api.Domain.TaskStatus;

namespace TaskManagement.UnitTests;

public sealed class TaskServiceTests
{
    private static TaskDbContext CreateDb() => new(new DbContextOptionsBuilder<TaskDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task Create_trims_title_and_assigns_owner()
    {
        await using var db = CreateDb(); var userId = Guid.NewGuid();
        var service = new TaskService(db, TimeProvider.System);
        var result = await service.CreateAsync(userId, new CreateTaskRequest("  Test  ", null), default);
        Assert.Equal("Test", result.Title); Assert.Equal(userId, result.UserId); Assert.Equal(TaskStatus.Pending, result.Status);
    }

    [Fact]
    public async Task Get_does_not_return_another_users_task()
    {
        await using var db = CreateDb(); var owner = Guid.NewGuid();
        var created = await new TaskService(db, TimeProvider.System).CreateAsync(owner, new CreateTaskRequest("Private", null), default);
        Assert.Null(await new TaskService(db, TimeProvider.System).GetAsync(Guid.NewGuid(), created.Id, default));
    }

    [Fact]
    public async Task List_filters_status_due_date_and_paginates()
    {
        await using var db = CreateDb(); var owner = Guid.NewGuid(); var now = DateTimeOffset.UtcNow;
        db.Tasks.AddRange(
            Item(owner, "A", TaskStatus.Pending, now.AddDays(1)), Item(owner, "B", TaskStatus.Pending, now.AddDays(2)),
            Item(owner, "C", TaskStatus.Completed, now.AddDays(1)), Item(Guid.NewGuid(), "Other", TaskStatus.Pending, now.AddDays(1)));
        await db.SaveChangesAsync();
        var result = await new TaskService(db, TimeProvider.System).ListAsync(owner, TaskStatus.Pending, now, now.AddDays(3), 2, 1, default);
        Assert.Single(result.Items); Assert.Equal(2, result.TotalCount); Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task Partial_update_changes_only_supplied_fields()
    {
        await using var db = CreateDb(); var owner = Guid.NewGuid(); var service = new TaskService(db, TimeProvider.System);
        var created = await service.CreateAsync(owner, new CreateTaskRequest("Original", "Keep"), default);
        var request = new UpdateTaskRequest { Status = TaskStatus.Completed, SuppliedProperties = new HashSet<string> { "status" } };
        var updated = await service.UpdateAsync(owner, created.Id, request, default);
        Assert.Equal("Original", updated!.Title); Assert.Equal("Keep", updated.Description); Assert.Equal(TaskStatus.Completed, updated.Status);
    }

    [Fact]
    public async Task Delete_enforces_ownership()
    {
        await using var db = CreateDb(); var owner = Guid.NewGuid(); var service = new TaskService(db, TimeProvider.System);
        var created = await service.CreateAsync(owner, new CreateTaskRequest("Private", null), default);
        Assert.False(await service.DeleteAsync(Guid.NewGuid(), created.Id, default)); Assert.True(await service.DeleteAsync(owner, created.Id, default));
    }

    private static TaskItem Item(Guid user, string title, TaskStatus status, DateTimeOffset due) => new()
    { Id = Guid.NewGuid(), Title = title, UserId = user, Status = status, DueDate = due, CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow };
}
