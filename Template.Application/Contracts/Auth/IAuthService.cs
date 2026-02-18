using Template.Application.Common.Results;
using Template.Application.DTOs.Auth;

namespace Template.Application.Contracts.Auth;

public interface IAuthService
{
    Task<Result<string>> RegisterAsync(string email, string password);
    Task<Result<AuthTokensResponse>> LoginAsync(string email, string password);
    Task<Result<AuthTokensResponse>> RefreshAsync(string refreshToken);
    Task<Result> LogoutAsync(string refreshToken);
}
