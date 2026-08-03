namespace TaskManagement.Api.Domain;
// Assumed existing identity model; only the relationship shape is included.
public sealed class User { public Guid Id { get; set; } public ICollection<TaskItem> Tasks { get; set; } = []; }
