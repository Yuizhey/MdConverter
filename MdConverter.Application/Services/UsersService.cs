using MdConverter.Core.Abstractions;
using MdConverter.Core.Models;

namespace MdConverter.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUserRepository userRepository;
    public UsersService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await userRepository.GetAllUsers();
    }
    
    public async Task<User> GetUserById(Guid id)
    {
        return await userRepository.GetUserById(id);
    }
    
    public async Task<User> GetUserByName(string name)
    {
        return await userRepository.GetUserByName(name);
    }
    
    public async Task<Guid> CreateUser(User user)
    {
        return await userRepository.CreateUser(user);
    }
    
    public async Task<Guid> UpdateUser(Guid id, string userName, string passwordHash)
    {
        return await userRepository.UpdateUser(id, userName, passwordHash);
    }
    
    public async Task<Guid> DeleteUser(Guid id)
    {
        return await userRepository.DeleteUser(id);
    }
}