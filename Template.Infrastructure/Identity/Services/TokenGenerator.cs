using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Template.Application.Contracts.Auth;

namespace Template.Infrastructure.Identity.Services;

public sealed class TokenGenerator(IConfiguration config) : ITokenGenerator
{
    public string CreateAccessToken(
        string userId,
        string email,
        string userName,
        IList<string> roles)
    {
        var jwt = config.GetSection("Jwt");

        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var key = jwt["SigningKey"]!;
        var minutes = int.Parse(jwt["AccessTokenMinutes"] ?? "30");

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, userId),
        new(JwtRegisteredClaimNames.Email, email),
        new(ClaimTypes.NameIdentifier, userId),
        new(ClaimTypes.Name, userName),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var creds = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
