using System.Text.RegularExpressions;

namespace Domain;

public record Email
{
    private Email() { }

    public Email(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentNullException(nameof(email));

        Value = email;
    }

    private string email = null!;
    public string Value
    {
        get => email;
        init
        {
            Regex regex = new("^\\S+@\\S+\\.\\S+$");
            if (!regex.IsMatch(value))
            {
                throw new ArgumentException("Invalid email format", "email");
            }

            email = value;
        }
    }
}
