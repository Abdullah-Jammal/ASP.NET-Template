using MediatR;
using Template.Application.Common.Results;

namespace Template.Application.Features.Auth.Register.Commands;

public sealed record RegisterCommand(
    string Email,
    string Password
) : IRequest<Result<string>>;
