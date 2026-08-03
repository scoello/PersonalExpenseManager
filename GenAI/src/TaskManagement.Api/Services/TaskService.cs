using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Contracts;
using TaskManagement.Api.Data;
using TaskManagement.Api.Domain;
using TaskStatus = TaskManagement.Api.Domain.TaskStatus;
namespace TaskManagement.Api.Services;
public sealed class TaskService(TaskDbContext db, TimeProvider clock) : ITaskService
{
    public async Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct)
    {
        var now = clock.GetUtcNow();
        var item = new TaskItem { Id = Guid.NewGuid(), Title = request.Title.Trim(), Description = request.Description,
            Status = request.Status, DueDate = request.DueDate, UserId = userId, CreatedAt = now, UpdatedAt = now };
        db.Tasks.Add(item); await db.SaveChangesAsync(ct); return Map(item);
    }
    public async Task<PagedResponse<TaskResponse>> ListAsync(Guid userId, TaskStatus? status, DateTimeOffset? dueFrom, DateTimeOffset? dueTo, int page, int pageSize, CancellationToken ct)
    {
        var query = db.Tasks.AsNoTracking().Where(x => x.UserId == userId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (dueFrom.HasValue) query = query.Where(x => x.DueDate >= dueFrom);
        if (dueTo.HasValue) query = query.Where(x => x.DueDate <= dueTo);
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).Skip((page - 1) * pageSize)
            .Take(pageSize).Select(x => Map(x)).ToListAsync(ct);
        return new(items, page, pageSize, total, (int)Math.Ceiling(total / (double)pageSize));
    }
    public async Task<TaskResponse?> GetAsync(Guid userId, Guid taskId, CancellationToken ct)
    { var item = await Owned(userId, taskId).AsNoTracking().SingleOrDefaultAsync(ct); return item is null ? null : Map(item); }
    public async Task<TaskResponse?> UpdateAsync(Guid userId, Guid taskId, UpdateTaskRequest request, CancellationToken ct)
    {
        var item = await Owned(userId, taskId).SingleOrDefaultAsync(ct); if (item is null) return null;
        if (request.SuppliedProperties.Contains("title")) item.Title = request.Title!.Trim();
        if (request.SuppliedProperties.Contains("description")) item.Description = request.Description;
        if (request.SuppliedProperties.Contains("status")) item.Status = request.Status!.Value;
        if (request.SuppliedProperties.Contains("dueDate")) item.DueDate = request.DueDate;
        item.UpdatedAt = clock.GetUtcNow(); await db.SaveChangesAsync(ct); return Map(item);
    }
    public async Task<bool> DeleteAsync(Guid userId, Guid taskId, CancellationToken ct)
    { var item = await Owned(userId, taskId).SingleOrDefaultAsync(ct); if (item is null) return false; db.Tasks.Remove(item); await db.SaveChangesAsync(ct); return true; }
    private IQueryable<TaskItem> Owned(Guid userId, Guid taskId) => db.Tasks.Where(x => x.Id == taskId && x.UserId == userId);
    private static TaskResponse Map(TaskItem x) => new(x.Id, x.Title, x.Description, x.Status, x.DueDate, x.UserId, x.CreatedAt, x.UpdatedAt);
}
