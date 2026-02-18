using MediatR;
using Template.Application.Common.Results;
using Template.Application.Contracts.Auth;
using Template.Application.DTOs.Auth;

internal sealed class RefreshTokenCommandHandler(
    IAuthService authService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokensResponse>>
{
    public Task<Result<AuthTokensResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
        => authService.RefreshAsync(request.RefreshToken);
}
