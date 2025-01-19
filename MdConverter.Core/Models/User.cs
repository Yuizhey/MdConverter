namespace MdConverter.Core.Models;

public class User
{
    public const  int MAX_PARAMETER_LENGTH = 30;
    public const int MIN_PARAMETER_LENGTH = 3;
    private User(Guid id, string name, string passwordHash)
    {
        Id = id;
        Name = name;
        PasswordHash = passwordHash;
    }
    public Guid Id { get; }
    public string Name { get; }
    public string PasswordHash { get; set; }

    public static (User user, string error) Create(Guid id, string name, string passwordHash)
    {
        var error = string.Empty;
        if (string.IsNullOrWhiteSpace(name) || name.Length > MAX_PARAMETER_LENGTH || name.Length < MIN_PARAMETER_LENGTH)
        {
            error = "Name must be between 3 and 30 characters";
        }
        var user = new User(id, name, passwordHash);
        return (user, error);
    }
}