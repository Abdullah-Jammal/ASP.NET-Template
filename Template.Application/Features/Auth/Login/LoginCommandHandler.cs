using MediatR;
using Template.Application.Common.Results;
using Template.Application.Contracts.Auth;
using Template.Application.DTOs.Auth;

namespace Template.Application.Features.Auth.Login;

internal sealed class LoginCommandHandler(IAuthService authService)
    : IRequestHandler<LoginCommand, Result<AuthTokensResponse>>
{
    public Task<Result<AuthTokensResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authService.LoginAsync(request.Email, request.Password);
}
