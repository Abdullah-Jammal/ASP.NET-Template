using Microsoft.AspNetCore.Identity;

namespace Template.Infrastructure.Identity.Entity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
