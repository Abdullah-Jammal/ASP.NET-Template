using MediatR;
using Template.Application.Common.Results;
using Template.Application.DTOs.Auth;

namespace Template.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthTokensResponse>>;
