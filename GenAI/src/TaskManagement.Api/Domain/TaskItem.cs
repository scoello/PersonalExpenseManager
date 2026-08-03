namespace TaskManagement.Api.Domain;
public sealed class TaskItem
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
