using MediatR;
using Template.Application.Common.Results;
using Template.Application.Contracts.Auth;

namespace Template.Application.Features.Auth.Logout;

internal sealed class LogoutCommandHandler(
    IAuthService authService)
    : IRequestHandler<LogoutCommand, Result>
{
    public Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
        => authService.LogoutAsync(request.RefreshToken);
}
