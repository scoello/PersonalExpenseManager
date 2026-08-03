using TaskManagement.Api.Contracts;
using TaskStatus = TaskManagement.Api.Domain.TaskStatus;
namespace TaskManagement.Api.Services;
public interface ITaskService
{
    Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct);
    Task<PagedResponse<TaskResponse>> ListAsync(Guid userId, TaskStatus? status, DateTimeOffset? dueFrom, DateTimeOffset? dueTo, int page, int pageSize, CancellationToken ct);
    Task<TaskResponse?> GetAsync(Guid userId, Guid taskId, CancellationToken ct);
    Task<TaskResponse?> UpdateAsync(Guid userId, Guid taskId, UpdateTaskRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid userId, Guid taskId, CancellationToken ct);
}
