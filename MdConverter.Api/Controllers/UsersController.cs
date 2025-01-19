using MdConverter.Api.RequestModels;
using MdConverter.Api.ResponseModels;
using MdConverter.Core.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace MdConverter.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService usersService;
    public UsersController(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
    {
        var users = await usersService.GetAllUsers();
        var result = users.Select(user => new UserResponse(user.Id, user.Name, user.PasswordHash));
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
    {
        var user = await usersService.GetUserById(id);
        var result = new UserResponse(user.Id, user.Name, user.PasswordHash);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<UserResponse>> GetUserByName(string name)
    {
        var user = await usersService.GetUserByName(name);
        var result = new UserResponse(user.Id, user.Name, user.PasswordHash);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateUser([FromBody]UserRequest userRequest)
    {
        var (user, error) = Core.Models.User.Create(
            Guid.NewGuid(),
            userRequest.name,
            userRequest.password);
        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }
        
        var userId = await usersService.CreateUser(user);
        
        return Ok(userId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UserRequest userRequest)
    {
        var userId = await usersService.UpdateUser(id,userRequest.name,userRequest.password);
        return Ok(userId);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteUser(Guid id)
    {
        var userId = await usersService.DeleteUser(id);
        return Ok(userId);
    }
}