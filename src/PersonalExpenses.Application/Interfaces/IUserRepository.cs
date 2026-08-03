using PersonalExpenses.Domain.Entities;
namespace PersonalExpenses.Application;
public interface IUserRepository
{
    Task<AppUser?> FindAsync(string username, CancellationToken cancellationToken);
    Task<IReadOnlyList<AppUser>> ListAsync(CancellationToken cancellationToken);
    Task AddAsync(AppUser user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
