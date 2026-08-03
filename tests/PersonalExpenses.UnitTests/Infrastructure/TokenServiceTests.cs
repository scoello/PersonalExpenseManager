using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using PersonalExpenses.Domain.Constants;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure;

namespace PersonalExpenses.UnitTests.Infrastructure;
public sealed class TokenServiceTests
{
    private static TokenService CreateService()
    {
        return new(
                new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { { "Jwt:Key", "unit-test-key-that-is-longer-than-32-characters" } })
                .Build()
            );
    }

    [Fact]
    public void Create_emits_identity_role_and_expiration_claims()
    {
        //Arrange
        var user = new AppUser("admin", "hash", Roles.Admin);

        //Act
        var token = new JwtSecurityTokenHandler().ReadJwtToken(CreateService().Create(user));

        //Assert
        Assert.Multiple(() =>
        {
            Assert.Equal(user.Id.ToString(), token.Subject);
            Assert.Equal("admin", token.Claims.Single(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value);
            Assert.Equal(Roles.Admin, token.Claims.Single(x => x.Type == ClaimTypes.Role).Value);
            Assert.True(token.ValidTo > DateTime.UtcNow.AddHours(7));
        });
    }

    [Fact]
    public void Create_throws_when_key_is_missing() 
    {
        //Arrange
        var service = new TokenService(new ConfigurationBuilder().Build());

        //Act
        var exception = Assert.Throws<InvalidOperationException>(() => service.Create(new AppUser("user", "hash")));

        //Assert
        Assert.Equal("Jwt:Key is missing.", exception.Message);
    }

}
