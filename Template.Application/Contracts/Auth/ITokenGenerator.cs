
namespace Template.Application.Contracts.Auth;

public interface ITokenGenerator
{
    string CreateAccessToken(
    string userId,
    string email,
    string userName,
    IList<string> roles);
    string CreateRefreshToken();
}
