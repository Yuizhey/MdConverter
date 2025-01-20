using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MdConverter.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MdConverter.Application.Services;

public class JwtService
{
    private readonly IOptions<AuthSettings> options;
    public JwtService(IOptions<AuthSettings> options)
    {
        this.options = options;
    }
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("UserName", user.Name),
            new Claim("id", user.Id.ToString()),
        };
        var token = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(options.Value.Expires),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}