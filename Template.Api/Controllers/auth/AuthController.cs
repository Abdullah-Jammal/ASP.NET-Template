using MediatR;
using Microsoft.AspNetCore.Mvc;
using Template.Api.Common.Middleware;
using Template.Application.Features.Auth.Login;
using Template.Application.Features.Auth.Logout;
using Template.Application.Features.Auth.Register.Commands;

namespace Template.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "auth")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(this);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(this);
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(this);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command)
    {
        var result = await sender.Send(command);
        return result.ToActionResult(this);
    }
}
