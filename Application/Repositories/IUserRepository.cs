using Domain;
using System.Linq.Expressions;

namespace Application.Repositories;

public interface IUserRepository
{
    Task CreateAsync(IEnumerable<User> users);
    Task DeleteAsync(IEnumerable<User> users);
    Task UpdateAsync(IEnumerable<User> users);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(Email email);
    Task<IList<User>> TakeUsers<T>(int count, Expression<Func<User, T>> keySelector);
    Task<int> GetUsersCount();
}
