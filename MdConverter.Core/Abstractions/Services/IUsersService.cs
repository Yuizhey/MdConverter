using MdConverter.Core.Models;

namespace MdConverter.Core.Abstractions.Services;

public interface IUsersService
{
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(Guid id);
    Task<User> GetUserByName(string name);
    Task<Guid> CreateUser(User user);
    Task<Guid> UpdateUser(Guid id, string userName, string passwordHash);
    Task<Guid> DeleteUser(Guid id);
}