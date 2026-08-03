using PersonalExpenses.Domain.Constants;
using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.UnitTests.Domain;
public sealed class AppUserTests
{
    [Fact] 
    public void Constructor_normalizes_username_and_assigns_default_role() 
    {
        //Arrange

        //Act
        var user = new AppUser(" Admin.User ", "hash");

        //Assert
        Assert.Multiple(() =>
        {
            Assert.NotEqual(Guid.Empty, user.Id);
            Assert.Equal("admin.user", user.Username);
            Assert.Equal("hash", user.PasswordHash);
            Assert.Equal(Roles.User, user.Role);
        });
    }

    [Fact]
    public void Constructor_accepts_admin_role()
    {
        //Arrange

        //Act
        var user = new AppUser("admin", "hash", Roles.Admin);

        //Assert
        Assert.Equal(Roles.Admin, user.Role);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_rejects_blank_username(string username)
    {
        //Arrange

        //Act
        var exception = Assert.Throws<ArgumentException>(() => new AppUser(username, "hash"));

        //Assert
        Assert.Equal("Username is required.", exception.Message);
    }
}
