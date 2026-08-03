using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskStatus = TaskManagement.Api.Domain.TaskStatus;
namespace TaskManagement.Api.Contracts;
public sealed record CreateTaskRequest(
    [property: Required, StringLength(200, MinimumLength = 1)] string Title,
    string? Description, TaskStatus Status = TaskStatus.Pending, DateTimeOffset? DueDate = null);
public sealed class UpdateTaskRequest
{
    [StringLength(200, MinimumLength = 1)] public string? Title { get; init; }
    public string? Description { get; init; }
    public TaskStatus? Status { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    [JsonIgnore] public ISet<string> SuppliedProperties { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}
public sealed record TaskResponse(Guid Id, string Title, string? Description, TaskStatus Status,
    DateTimeOffset? DueDate, Guid UserId, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
