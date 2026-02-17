using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Infrastructure.Identity;

namespace Template.Infrastructure.Configurations.Auth;

public static class IdentitySchemaConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>().ToTable("Users", "auth");
        builder.Entity<IdentityRole>().ToTable("Roles", "auth");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "auth");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "auth");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "auth");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "auth");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "auth");
    }
}
