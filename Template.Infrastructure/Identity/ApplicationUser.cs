using Microsoft.AspNetCore.Identity;

namespace Template.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
