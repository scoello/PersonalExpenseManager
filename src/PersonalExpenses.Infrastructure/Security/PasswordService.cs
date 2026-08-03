using System.Security.Cryptography;
using PersonalExpenses.Application;
namespace PersonalExpenses.Infrastructure;
public sealed class PasswordService : IPasswordService
{
    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);

        return Convert.ToBase64String(salt.Concat(hash).ToArray());
    }
    public bool Verify(string password, string encoded)
    {
        try 
        { 
            var bytes = Convert.FromBase64String(encoded);
            return CryptographicOperations.FixedTimeEquals(Rfc2898DeriveBytes.Pbkdf2(password, bytes[..16], 100_000, HashAlgorithmName.SHA256, 32), bytes[16..]);
        }
        catch 
        { 
            return false; 
        }
    }
}
