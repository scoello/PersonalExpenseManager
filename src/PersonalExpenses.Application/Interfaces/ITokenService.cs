using PersonalExpenses.Domain.Entities;
namespace PersonalExpenses.Application;
public interface ITokenService
{
    string Create(AppUser user);
}
