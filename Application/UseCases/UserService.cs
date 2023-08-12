using Application.Repositories;
using Domain;
using System.Text;
namespace Application.UseCases;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Block(IEnumerable<string> userIds)
    {
        var users = await FindUsersAsync(userIds);
        CheckForErrors(users.Item2);
        foreach (var user in users.Item1) user.ChangeStatus(true);
        await _userRepository.UpdateAsync(users.Item1);
    }

    public async Task Delete(IEnumerable<string> userIds)
    {
        var users = await FindUsersAsync(userIds);
        CheckForErrors(users.Item2);
        await _userRepository.DeleteAsync(users.Item1);
    }

    public async Task Unblock(IEnumerable<string> userIds)
    {
        var users = await FindUsersAsync(userIds);
        CheckForErrors(users.Item2);
        foreach (var user in users.Item1) user.ChangeStatus(false);
        await _userRepository.UpdateAsync(users.Item1);
    }

    public async Task Login(string email, string password)
    {
        User user = await _userRepository.GetByEmailAsync(new(email)) ?? throw new ArgumentException($"Cannot find a user with a specified email ({email})", nameof(email));
        if (user.IsBlocked) throw new InvalidOperationException("User is blocked! Unblock this user via another account.");
        if (user.Password != password) throw new ArgumentException($"Wrong password for user ({user.Id})", nameof(password));
        user.WriteLastLogin(DateTime.UtcNow);
        await _userRepository.UpdateAsync(new List<User>() { user });
    }

    public async Task Register(User user)
    {
        if (await _userRepository.GetByEmailAsync(user.Email) is not null) throw new ArgumentException($"User already exists ({user.Id})", nameof(user));
        await _userRepository.CreateAsync(new List<User>() { user });
    }

    private async Task<Tuple<IEnumerable<User>, string>> FindUsersAsync(IEnumerable<string> userIds)
    {
        StringBuilder errorIdsSb = new();
        List<User> users = new();
        foreach (var userId in userIds)
        {
            Guid.TryParse(userId, out var userIdGuid);
            User? user = await _userRepository.GetByIdAsync(userIdGuid);
            if (user is null)
            {
                if (errorIdsSb.Length != 0)
                    errorIdsSb.Append(", ");
                errorIdsSb.Append(userId);
            }
            else
            {
                users.Add(user);
            }
        }

        return new(users, errorIdsSb.ToString());
    }

    private void CheckForErrors(string userIds)
    {
        if (!string.IsNullOrEmpty(userIds)) throw new ArgumentException($"Cannot find users with a specified ids ({userIds})", nameof(userIds));
    }
}
