using MediatR;
using Template.Application.Common.Results;
using Template.Application.Contracts.Auth;

namespace Template.Application.Features.Auth.Register.Commands;

internal sealed class RegisterCommandHandler(
    IAuthService authService
) : IRequestHandler<RegisterCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(
            request.Email,
            request.Password);
    }
}
