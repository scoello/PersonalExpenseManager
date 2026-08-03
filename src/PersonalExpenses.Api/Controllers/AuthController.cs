using Microsoft.AspNetCore.Mvc;
using PersonalExpenses.Application;
namespace PersonalExpenses.Api.Controllers;

[ApiController, Route("api/auth")]
public sealed class AuthController(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService) : ControllerBase
{
    /// <summary>
    /// Authenticate a user
    /// </summary>
    /// <param name="request">LoginRequest</param>
    /// <param name="ct"></param>
    /// <returns>Unauthorized or OK with the token and user information</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await userRepository.FindAsync(request.Username, ct);

        if (user is null || !passwordService.Verify(request.Password, user.PasswordHash))
            return Unauthorized();

        var loginResponse = new LoginResponse(tokenService.Create(user), user.Username, user.Role);
        return Ok(loginResponse);
    }
}
