using System.Text.Json;
using System.Text.Json.Serialization;
using TaskStatus = TaskManagement.Api.Domain.TaskStatus;
namespace TaskManagement.Api.Contracts;
public sealed class UpdateTaskRequestJsonConverter : JsonConverter<UpdateTaskRequest>
{
    public override UpdateTaskRequest Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        return new UpdateTaskRequest {
            Title = Read<string>(root, "title", options), Description = Read<string>(root, "description", options),
            Status = Read<TaskStatus?>(root, "status", options), DueDate = Read<DateTimeOffset?>(root, "due_date", options),
            SuppliedProperties = root.EnumerateObject().Select(x => x.Name.Replace("_", "", StringComparison.Ordinal)).ToHashSet(StringComparer.OrdinalIgnoreCase)
        };
    }
    private static T? Read<T>(JsonElement root, string name, JsonSerializerOptions options) =>
        root.TryGetProperty(name, out var value) ? value.Deserialize<T>(options) : default;
    public override void Write(Utf8JsonWriter writer, UpdateTaskRequest value, JsonSerializerOptions options) => throw new NotSupportedException();
}
