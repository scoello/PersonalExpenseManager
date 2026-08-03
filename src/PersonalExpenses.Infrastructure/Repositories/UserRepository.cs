using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Application;
using PersonalExpenses.Domain.Entities;
namespace PersonalExpenses.Infrastructure;
public sealed class UserRepository(ExpensesDbContext dbContext) : IUserRepository
{
    public Task<AppUser?> FindAsync(string username, CancellationToken cancellationToken)
    {
        return dbContext.Users.SingleOrDefaultAsync(x => x.Username == username.Trim().ToLower(), cancellationToken);
    }

    public async Task<IReadOnlyList<AppUser>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Users.OrderBy(x => x.Username)
                                    .AsNoTracking()
                                    .ToListAsync(cancellationToken);
    }

    public Task AddAsync(AppUser user, CancellationToken cancellationToken)
    {
        return dbContext.Users.AddAsync(user, cancellationToken)
                              .AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
