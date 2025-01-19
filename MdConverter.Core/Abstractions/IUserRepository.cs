using MdConverter.Core.Models;

namespace MdConverter.Core.Abstractions;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(Guid id);
    Task<User> GetUserByName(string name);
    Task<Guid> CreateUser(User user);
    Task<Guid> UpdateUser(Guid id, string name, string passwordHash);
    Task<Guid> DeleteUser(Guid id);
}