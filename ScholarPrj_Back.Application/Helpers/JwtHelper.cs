using Microsoft.IdentityModel.Tokens;
using ScholarPrj_Back.Domain.Responses.Auth;
using ScholarPrj_Back.Infrastructure.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class JwtHelper
{
    public static string GenerateToken(LoginResponse user, JwtSettings jwt)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserDetail.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserDetail.UserName),
            new Claim(ClaimTypes.Role, user.UserDetail.Role.Name)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt.Key)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwt.ExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}