namespace Template.Application.DTOs.Auth;

public sealed record AuthTokensResponse(
    string AccessToken,
    string RefreshToken
);
