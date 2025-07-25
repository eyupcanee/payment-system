using System.ComponentModel.DataAnnotations.Schema;
using Identity.Domain.Entities;
using Identity.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) {}
    
    
    public DbSet<AppUser> Users { get; set; }
    public DbSet<AppRole> Roles { get; set; }
    public DbSet<AppUserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUserRole>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<AppUserRole>().HasOne(x => x.User).WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);
        modelBuilder.Entity<AppUserRole>().HasOne(x => x.Role).WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
        modelBuilder.Entity<RolePermission>().HasOne(x => x.Role).WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);
        modelBuilder.Entity<RolePermission>().HasOne(x => x.Permission).WithMany()
            .HasForeignKey(p => p.PermissionId);

        modelBuilder.Entity<AppUser>().ToTable("users");
        modelBuilder.Entity<AppRole>().ToTable("roles");
        modelBuilder.Entity<AppUserRole>().ToTable("user_roles");
        modelBuilder.Entity<Permission>().ToTable("permissions");
        modelBuilder.Entity<RolePermission>().ToTable("role_permissions");
        
        IdentitySeeder.Seed(modelBuilder);
    }
}