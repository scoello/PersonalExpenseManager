using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalExpenses.Application;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Domain.Constants;
namespace PersonalExpenses.Api.Controllers;

[ApiController, Authorize(Roles = Roles.Admin), Route("api/users")]
public sealed class UsersController(IUserRepository userRepository, IPasswordService passwordService) : ControllerBase
{
    /// <summary>
    /// Get a list of users
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>A list of users</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> List(CancellationToken ct)
    {
        var list = await userRepository.ListAsync(ct);
        return Ok(list.Select(x => new UserDto(x.Id, x.Username, x.Role)));
    }
    
    /// <summary>
    /// Create a user
    /// </summary>
    /// <param name="request">the user request</param>
    /// <param name="ct"></param>
    /// <returns>201</returns>
    [HttpPost] 
    public async Task<ActionResult<UserDto>> Create(CreateUserRequest request, CancellationToken ct) 
    { 
        if (string.IsNullOrWhiteSpace(request.Username) || request.Password.Length < 8) 
            return BadRequest("Username is required and password must contain at least 8 characters."); 
        
        if (await userRepository.FindAsync(request.Username, ct) is not null) 
            return Conflict("Username already exists."); 
        
        var user = new AppUser(request.Username, passwordService.Hash(request.Password)); 
        await userRepository.AddAsync(user, ct); 
        await userRepository.SaveChangesAsync(ct); 
        
        return Created($"/api/users/{user.Id}", new UserDto(user.Id, user.Username, user.Role)); 
    }
}