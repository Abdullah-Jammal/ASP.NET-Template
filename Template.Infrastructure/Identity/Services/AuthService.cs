using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Template.Application.Common.Errors;
using Template.Application.Common.Results;
using Template.Application.Contracts.Auth;
using Template.Application.DTOs.Auth;
using Template.Infrastructure.Identity.Entity;
using Template.Infrastructure.Persistence;

namespace Template.Infrastructure.Identity.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext,
    SignInManager<ApplicationUser> signInManager,
    ITokenGenerator tokenGenerator
) : IAuthService
{
    public async Task<Result<string>> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };

        var create = await userManager.CreateAsync(user, password);
        if (!create.Succeeded)
        {
            var errors = create.Errors.Select(e =>
                new Error("identity.create_failed", e.Description, ErrorType.Validation)).ToList();
            return Result<string>.Failure(errors);
        }

        return Result<string>.Success("User created successfully");
    }

    public async Task<Result<AuthTokensResponse>> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<AuthTokensResponse>.Failure(
                new Error("auth.invalid_credentials", "Invalid email or password.", ErrorType.Unauthorized));

        var check = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!check.Succeeded)
            return Result<AuthTokensResponse>.Failure(
                new Error("auth.invalid_credentials", "Invalid email or password.", ErrorType.Unauthorized));

        var roles = await userManager.GetRolesAsync(user);

        var accessToken = tokenGenerator.CreateAccessToken(
            user.Id,
            user.Email!,
            user.UserName ?? user.Email!,
            roles
        );
        var refreshToken = tokenGenerator.CreateRefreshToken();
        var refreshTokenHash = HashToken(refreshToken);

        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        dbContext.RefreshTokens.Add(entity);
        await dbContext.SaveChangesAsync();

        return Result<AuthTokensResponse>.Success(
            new AuthTokensResponse(accessToken, refreshToken));
    }
    public async Task<Result<AuthTokensResponse>> RefreshAsync(string refreshToken)
    {
        var hash = HashToken(refreshToken);

        var stored = await dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == hash &&
                !x.IsRevoked);

        if (stored is null || stored.ExpiresAtUtc < DateTime.UtcNow)
            return Result<AuthTokensResponse>.Failure(
                new Error("auth.invalid_refresh", "Invalid refresh token", ErrorType.Unauthorized));

        stored.IsRevoked = true;
        stored.RevokedAtUtc = DateTime.UtcNow;

        var newRefresh = tokenGenerator.CreateRefreshToken();
        var newHash = HashToken(newRefresh);

        var newEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = stored.UserId,
            TokenHash = newHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };

        dbContext.RefreshTokens.Add(newEntity);

        var roles = await userManager.GetRolesAsync(stored.User);

        var accessToken = tokenGenerator.CreateAccessToken(
            stored.User.Id,
            stored.User.Email!,
            stored.User.UserName!,
            roles);

        await dbContext.SaveChangesAsync();

        return Result<AuthTokensResponse>.Success(
            new AuthTokensResponse(accessToken, newRefresh));
    }

    public async Task<Result> LogoutAsync(string refreshToken)
    {
        var hash = HashToken(refreshToken);

        var stored = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.TokenHash == hash &&
                !x.IsRevoked);

        if (stored is null)
            return Result.Success();

        stored.IsRevoked = true;
        stored.RevokedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return Result.Success();
    }


    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
