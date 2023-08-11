namespace Presentation.Models;

public class UserViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = null!;
    public bool IsBlocked { get; set; } = true;
    public string Status => IsBlocked ? "Blocked" : "Active";
    public bool IsSelected { get; set; } = false;
    public string CreatedAt { get; set; } = string.Empty;
    public string LastLoginAt { get; set; } = string.Empty;
}
