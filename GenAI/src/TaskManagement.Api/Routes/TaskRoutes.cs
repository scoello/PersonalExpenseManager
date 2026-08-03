using System.ComponentModel.DataAnnotations;
using TaskManagement.Api.Contracts;
using TaskManagement.Api.Dependencies;
using TaskManagement.Api.Services;
using TaskStatus = TaskManagement.Api.Domain.TaskStatus;
namespace TaskManagement.Api.Routes;
public static class TaskRoutes
{
    public static IEndpointRouteBuilder MapTaskRoutes(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/tasks").WithTags("Tasks").RequireAuthorization();
        group.MapPost("/", Create).WithName("CreateTask").Produces<TaskResponse>(201).ProducesValidationProblem();
        group.MapGet("/", List).WithName("ListTasks").Produces<PagedResponse<TaskResponse>>();
        group.MapGet("/{taskId:guid}", Get).WithName("GetTask").Produces<TaskResponse>().ProducesProblem(404);
        group.MapPut("/{taskId:guid}", Update).WithName("UpdateTask").Produces<TaskResponse>().ProducesValidationProblem().ProducesProblem(404);
        group.MapPatch("/{taskId:guid}", Update).WithName("PatchTask").Produces<TaskResponse>().ProducesValidationProblem().ProducesProblem(404);
        group.MapDelete("/{taskId:guid}", Delete).WithName("DeleteTask").Produces(204).ProducesProblem(404);
        return endpoints;
    }
    private static async Task<IResult> Create(CreateTaskRequest request, ITaskService service, ICurrentUser user, CancellationToken ct)
    {
        var errors = Validate(request); if (errors.Count > 0) return Results.ValidationProblem(errors);
        var result = await service.CreateAsync(user.UserId, request, ct); return Results.CreatedAtRoute("GetTask", new { taskId = result.Id }, result);
    }
    private static async Task<IResult> List(ITaskService service, ICurrentUser user, TaskStatus? status, DateTimeOffset? dueFrom, DateTimeOffset? dueTo, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var errors = new Dictionary<string, string[]>();
        if (page < 1) errors["page"] = ["Page must be at least 1."];
        if (pageSize is < 1 or > 100) errors["pageSize"] = ["Page size must be between 1 and 100."];
        if (dueFrom > dueTo) errors["dueDate"] = ["dueFrom must be before or equal to dueTo."];
        return errors.Count > 0 ? Results.ValidationProblem(errors) : Results.Ok(await service.ListAsync(user.UserId, status, dueFrom, dueTo, page, pageSize, ct));
    }
    private static async Task<IResult> Get(Guid taskId, ITaskService service, ICurrentUser user, CancellationToken ct) =>
        await service.GetAsync(user.UserId, taskId, ct) is { } item ? Results.Ok(item) : NotFound();
    private static async Task<IResult> Update(Guid taskId, UpdateTaskRequest request, ITaskService service, ICurrentUser user, CancellationToken ct)
    {
        var errors = Validate(request); if (request.SuppliedProperties.Count == 0) errors["body"] = ["At least one field must be supplied."];
        if (request.SuppliedProperties.Contains("title") && string.IsNullOrWhiteSpace(request.Title)) errors["title"] = ["Title is required."];
        if (request.SuppliedProperties.Contains("status") && request.Status is null) errors["status"] = ["Status cannot be null."];
        if (errors.Count > 0) return Results.ValidationProblem(errors);
        return await service.UpdateAsync(user.UserId, taskId, request, ct) is { } item ? Results.Ok(item) : NotFound();
    }
    private static async Task<IResult> Delete(Guid taskId, ITaskService service, ICurrentUser user, CancellationToken ct) =>
        await service.DeleteAsync(user.UserId, taskId, ct) ? Results.NoContent() : NotFound();
    private static Dictionary<string, string[]> Validate(object request)
    {
        var results = new List<ValidationResult>(); Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results.SelectMany(x => x.MemberNames.DefaultIfEmpty("body"), (x, member) => (member, x.ErrorMessage!)).GroupBy(x => x.member)
            .ToDictionary(x => char.ToLowerInvariant(x.Key[0]) + x.Key[1..], x => x.Select(v => v.Item2).ToArray());
    }
    private static IResult NotFound() => Results.Problem(statusCode: 404, title: "Task not found", detail: "The task does not exist or is not owned by the current user.");
}
