namespace MdConverter.DataAccess.Entities;

public class UserEntity
{
    public Guid Id { get; }
    public string Name { get; }
    public string PasswordHash { get; }
}