using System.Security.Claims;
namespace TaskManagement.Api.Dependencies;
public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? accessor.HttpContext?.User.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id
                : throw new UnauthorizedAccessException("The authenticated user identifier is missing or invalid.");
        }
    }
}
