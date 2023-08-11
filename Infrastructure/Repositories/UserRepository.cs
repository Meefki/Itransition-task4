using Application.Repositories;
using Domain;
using Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext userDbContext)
    {
        _context = userDbContext;
    }

    public async Task CreateAsync(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            _context.Users.Add(user);
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            _context.Users.Remove(user);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(Email email)
    {
        return await Task.Run(() => _context.Users.FirstOrDefault(u => u.Email.Value == email.Value));
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await Task.Run(() => _context.Users.FirstOrDefault(x => x.Id == userId));
    }

    public async Task<int> GetUsersCount()
    {
        return await Task.Run(() => _context.Users.Count());
    }

    public async Task<IList<User>> TakeUsers<T>(int count, Expression<Func<User, T>> keySelector)
    {
        return await Task.Run(() => _context.Users.OrderBy(keySelector).Take(count).ToList());
    }

    public async Task UpdateAsync(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
    }
}
