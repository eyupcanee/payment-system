namespace Identity.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}