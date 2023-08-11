namespace Domain;

public class User
{
    public Guid Id { get; init; }
    public bool IsBlocked { get; private set; }
    public string Name { get; private set; }
    public string Password { get; private set; }
    public Email Email { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastLoginAt { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public User(
        Guid id,
        string name,
        string password,
        Email email)
    {
        Id = id;
        Name = name;
        Password = password;
        Email = email;

        CreatedAt = DateTime.Now;
        LastLoginAt = CreatedAt;
    }

    public void ChangeStatus(bool isBlocked)
    {
        IsBlocked = isBlocked;
    }

    public void WriteLastLogin(DateTime lastLoginAt)
    {
        if (lastLoginAt < LastLoginAt)
            throw new ArgumentException("Cannot write login date earlier than existed", nameof(lastLoginAt));

        LastLoginAt = lastLoginAt;
    }
}