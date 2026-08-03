using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Contracts;
using TaskManagement.Api.Data;
using TaskManagement.Api.Dependencies;
using TaskManagement.Api.Errors;
using TaskManagement.Api.Routes;
using TaskManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.SnakeCaseLower));
    options.SerializerOptions.Converters.Add(new UpdateTaskRequestJsonConverter());
});
builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is required.")));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAuthentication("ExistingAuthentication")
    .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>("ExistingAuthentication", _ => { });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();
app.MapTaskRoutes();
app.Run();

public partial class Program;
