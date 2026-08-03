using PersonalExpenses.Domain.Constants;

namespace PersonalExpenses.Domain.Entities;

public sealed class AppUser
{
    private AppUser() { }

    public AppUser(string username, string passwordHash, string role = Roles.User)
    {
        if (string.IsNullOrWhiteSpace(username)) 
            throw new ArgumentException("Username is required.");
        
        Id = Guid.NewGuid();
        Username = username.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
    }

    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = Roles.User;
}