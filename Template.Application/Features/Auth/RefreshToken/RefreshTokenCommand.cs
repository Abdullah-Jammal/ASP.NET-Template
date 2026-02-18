using MediatR;
using Template.Application.Common.Results;
using Template.Application.DTOs.Auth;

public sealed record RefreshTokenCommand(
    string RefreshToken)
    : IRequest<Result<AuthTokensResponse>>;
