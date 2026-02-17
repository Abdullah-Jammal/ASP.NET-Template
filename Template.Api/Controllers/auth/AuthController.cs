using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Template.Infrastructure.Identity;

namespace Template.Api.Controllers.auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User created");
    }
}
