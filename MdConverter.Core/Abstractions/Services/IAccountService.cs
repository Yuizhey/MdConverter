using MdConverter.Core.Models;

namespace MdConverter.Core.Abstractions.Services;

public interface IAccountService
{
    Task Register(string username, string password);
    Task<string>Login(string name, string passwordHash);
}