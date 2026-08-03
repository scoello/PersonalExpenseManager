using PersonalExpenses.Infrastructure;

namespace PersonalExpenses.UnitTests.Infrastructure;
public sealed class PasswordServiceTests
{
    private readonly PasswordService service = new();

    [Fact]
    public void Hash_and_verify_accept_correct_password() 
    {
        //Arrange

        //Act
        var hash = service.Hash("Password1!");

        //Assert
        Assert.True(service.Verify("Password1!",hash)); 
    }

    [Fact] 
    public void Verify_rejects_wrong_password()
    {
        //Arrange

        //Act
        var hash = service.Hash("Password1!");

        //Assert
        Assert.False(service.Verify("WrongPassword",hash)); 
    }

    [Fact]
    public void Hash_uses_a_unique_salt()
    {
        //Arrange

        //Act
        var hash1 = service.Hash("Password1!");
        var hash2 = service.Hash("Password1!");

        //Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_rejects_malformed_hash()
    {
        //Arrange

        //Act
        var result = service.Verify("Password1!", "not-base64");

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void Hash_rejects_blank_password()
    {
        //Arrange

        //Act
        var exception = Assert.Throws<ArgumentException>(() => service.Hash(" "));

        //Assert
        Assert.Equal("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'password')", exception.Message);
    }
}
