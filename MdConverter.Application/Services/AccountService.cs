using MdConverter.Core.Abstractions.Repositories;
using MdConverter.Core.Abstractions.Services;
using MdConverter.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace MdConverter.Application.Services;



public class AccountService : IAccountService
{
    private readonly IUserRepository userRepository;
    private readonly JwtService jwtService;
    public AccountService(IUserRepository userRepository, JwtService jwtService)
    {
        this.userRepository = userRepository;
        this.jwtService = jwtService;
    }

    public async Task Register(string username, string password)
    {
        var user = User.Create(Guid.NewGuid(), username,password).user;
        var passwordHash =new PasswordHasher<User>().HashPassword(user, password);
        user.PasswordHash = passwordHash;
        await userRepository.CreateUser(user);
    }
    
    public async Task<string> Login(string name, string passwordHash)
    {
        var user = await userRepository.GetUserByName(name);
        var result = new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash, passwordHash);
        if (result == PasswordVerificationResult.Success)
        {
            return jwtService.GenerateToken(user);
        }
        else
        {
            throw new ApplicationException("Wrong password");
        }
    }
}