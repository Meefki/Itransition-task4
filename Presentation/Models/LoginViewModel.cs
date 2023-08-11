using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class LoginViewModel
{
    public LoginViewModel()
    {
        Action = IdentityAction.Login;
        KeepLoggedIn = true;

        Email = Password = string.Empty;
    }

    [RegularExpression("^\\S+@\\S+\\.\\S+$")]
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public string? Name { get; set; }
    public bool KeepLoggedIn { get; set; }
    public IdentityAction Action { get; set; }
}
