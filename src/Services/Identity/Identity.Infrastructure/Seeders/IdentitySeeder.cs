using Common.Authorization;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Seeders;

public static class IdentitySeeder
{
    private static readonly Guid adminUserId = Guid.Parse("11111111-1111-1111-1111-111112221111");
    private static readonly Guid adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static void Seed(ModelBuilder modelBuilder)
    {
        var permissions = new List<Permission>
        {
            new() { Id = 1, Name = Permissions.Permission.Read },
            new() { Id = 2, Name = Permissions.Permission.Write },
            new() { Id = 3, Name = Permissions.Permission.Update },
            new() { Id = 4, Name = Permissions.Permission.Delete },
            new() { Id = 5, Name = Permissions.Payment.Read },
            new() { Id = 6, Name = Permissions.Payment.Write },
            new() { Id = 7, Name = Permissions.Payment.Update },
            new() { Id = 8, Name = Permissions.Payment.Delete },
        };
        modelBuilder.Entity<Permission>().HasData(permissions);
        
        modelBuilder.Entity<AppRole>().HasData(
            new AppRole { Id = adminRoleId, Name = "Admin" },
            new AppRole { Id = userRoleId, Name = "User" }
        );
        
        var adminRolePermissions = permissions.Select(p => new RolePermission { RoleId = adminRoleId, PermissionId = p.Id }).ToList();
        modelBuilder.Entity<RolePermission>().HasData(adminRolePermissions);

        modelBuilder.Entity<RolePermission>().HasData(
            new RolePermission { RoleId = userRoleId, PermissionId = 1 },
            new RolePermission { RoleId = userRoleId, PermissionId = 5 },
            new RolePermission { RoleId = userRoleId, PermissionId = 6 }
        );

        modelBuilder.Entity<AppUser>().HasData(new AppUser
        {
            Id = adminUserId,
            Email = "eyupcanee@gmail.com",
            PasswordHash = "AQAAAAIAAYagAAAAEOo2t9Ptm//TK+heykrn/UKejsSmGtfhAogweyleWyOpwb5HXy/wJuONZfQt2Dis6g=="
        });
        
        modelBuilder.Entity<AppUserRole>().HasData(new AppUserRole
        {
            UserId = adminUserId,
            RoleId = adminRoleId
        });
    }
}