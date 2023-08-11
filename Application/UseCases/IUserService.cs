using Domain;

namespace Application.UseCases;

public interface IUserService
{
    Task Register(User user);
    Task Delete(IEnumerable<string> userIds);
    Task Block(IEnumerable<string> userIds);
    Task Unblock(IEnumerable<string> userIds);
    Task Login(string email, string password);
}
