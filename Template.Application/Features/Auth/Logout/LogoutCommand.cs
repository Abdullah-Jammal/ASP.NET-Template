using MediatR;
using Template.Application.Common.Results;

namespace Template.Application.Features.Auth.Logout;

public sealed record LogoutCommand(
    string RefreshToken
) : IRequest<Result>;
