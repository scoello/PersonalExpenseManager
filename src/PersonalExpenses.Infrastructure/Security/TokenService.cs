using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalExpenses.Application;
using PersonalExpenses.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonalExpenses.Infrastructure;
public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public string Create(AppUser user)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");

        var claims = new[] 
            { 
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role) 
            };

        var token = new JwtSecurityToken(claims: claims, 
                                         expires: DateTime.UtcNow.AddHours(8),
                                         signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                                         SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
