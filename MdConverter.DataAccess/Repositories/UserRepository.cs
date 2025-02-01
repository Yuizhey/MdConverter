
using MdConverter.Core.Abstractions.Repositories;
using MdConverter.Core.Models;
using MdConverter.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace MdConverter.DataAccess.Repositories;



public class UserRepository : IUserRepository
{
    private readonly MdConverterDbContext context;
    public UserRepository(MdConverterDbContext context)
    {
        this.context = context;
    }

    public async Task<List<User>> GetAllUsers()
    {
        var userEntities = await context.Users.AsNoTracking().ToListAsync();
        var users = userEntities.Select(u => User.Create(u.Id,u.Name,u.PasswordHash).user).ToList();
        return users;
    }

    public async Task<User> GetUserById(Guid id)
    {
        var userEntity = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        var user = User.Create(userEntity.Id, userEntity.Name, userEntity.PasswordHash).user;
        return user;
    }
    
    public async Task<User?> GetUserByName(string name)
    {
        var userEntity = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == name);
        if (userEntity == null)
        {
            return null;
        }

        var user = User.Create(userEntity.Id, userEntity.Name, userEntity.PasswordHash).user;
        return user;
    }

    public async Task<Guid> CreateUser(User user)
    {
        var userEntity = new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            PasswordHash = user.PasswordHash,
        };
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        return userEntity.Id;
    }

    public async Task<Guid> UpdateUser(Guid id, string name, string passwordHash)
    {
        await context.Users.Where(i => i.Id == id)
            .ExecuteUpdateAsync(u => u
                .SetProperty(i => i.Name, name)
                .SetProperty(i => i.PasswordHash, passwordHash));
        await context.SaveChangesAsync();
        return id;
    }

    public async Task<Guid> DeleteUser(Guid id)
    {
        await context.Users.Where(i => i.Id == id).ExecuteDeleteAsync();
        await context.SaveChangesAsync();
        return id;
    }
}