using MdConverter.Api.Filters;
using MdConverter.Api.RequestModels;
using MdConverter.Application.Services;
using MdConverter.Core.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MdConverter.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService accountService;
    public AccountController(IAccountService accountService)
    {
        this.accountService = accountService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Register([FromBody]UserRequest userRequest)
    {
        var (user, error) = Core.Models.User.Create(
            Guid.NewGuid(),
            userRequest.name,
            userRequest.password);
        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }
        await accountService.Register(user.Name,user.PasswordHash);
        return Ok();
    }
    
    [HttpPost]
    public async Task<ActionResult> Login([FromBody]UserRequest userRequest)
    {
        var (user, error) = Core.Models.User.Create(
            Guid.NewGuid(),
            userRequest.name,
            userRequest.password);
        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }
        var token = await accountService.Login(user.Name, user.PasswordHash);
        HttpContext.Response.Cookies.Append("token", token);
        return Ok(token);
    }
    
    [HttpPost]
    [MyAuthorizeFilter]
    public IActionResult Logout()
    {
        if (!HttpContext.Request.Cookies.ContainsKey("token"))
        {
            return BadRequest(new { message = "Token not found" });
        }

        // Удаляем токен из cookies
        HttpContext.Response.Cookies.Delete("token");

        return Ok(new { message = "Successfully logged out" });
    }


    
    [MyAuthorizeFilter]
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        var items = new List<int>
        {
            1,
            2,
            3
        };

        return Ok(items);

    }
}