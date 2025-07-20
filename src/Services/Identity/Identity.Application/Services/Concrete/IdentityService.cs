using System.Diagnostics;
using Identity.Application.Security.Abstract;
using Identity.Application.Services.Abstract;
using Identity.Domain.Entities;
using Identity.Infrastructure;
using Infrastructure.Cache.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Services.Concrete;

public class IdentityService : IIdentityService
{
    private readonly IdentityDbContext _context;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<IdentityService> _logger;
    private readonly ActivitySource _activitySource = new ActivitySource("IdentityService.IdentityService");

    public IdentityService(IdentityDbContext context, IPasswordHasher<AppUser> passwordHasher,
        IConfiguration configuration, ITokenBlacklistService tokenBlacklistService, ITokenService tokenService, ILogger<IdentityService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _tokenBlacklistService = tokenBlacklistService;
        _tokenService = tokenService;
        _logger = logger;
    }

  

    public async Task<string> LoginAsync(string email, string password)
    {
        using var activity = _activitySource.StartActivity("LoginAsync");
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException();
            }
            
            var result = _passwordHasher.VerifyHashedPassword(user,user.PasswordHash, password);
            if (result != PasswordVerificationResult.Success)
            {
                _logger.LogWarning( "User login failed with wrong password. Email: {Email}", email);
                throw new KeyNotFoundException();
            }
            
            var permissions = GetUserPermissions(user);
            var token = _tokenService.GenerateJwtToken(user,permissions);
            
            _logger.LogInformation("User logged in successfully. Email: {Email} ",user.Email);
            return token;
        } catch (Exception ex)
        {
            _logger.LogError(ex, "User login failed. Email: {Email}", email);
            throw;
        }
    }

    public async Task LogoutAsync(string token)
    {
        using var activity = _activitySource.StartActivity("LogoutAsync");
        try
        {
            _logger.LogInformation("User logged out successfully.");
            await _tokenBlacklistService.AddToBlacklistAsync(token, TimeSpan.FromDays(1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User logout failed.");
            throw;
        }
    }

    public async Task<AppUser> RegisterAsync(string email, string password)
    {
        using var activity = _activitySource.StartActivity("RegisterAsync");
        try
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ArgumentException();
            }

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole == null)
            {
                throw new Exception("Internal Server Error");
            }
            
            using var transaction = await _context.Database.BeginTransactionAsync();

            var user = new AppUser { Id = Guid.NewGuid(), Email = email };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userRoleRelation = new AppUserRole { UserId = user.Id, RoleId = userRole.Id };
            await _context.UserRoles.AddAsync(userRoleRelation);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            
            _logger.LogInformation("User registered successfully. Email: {Email}", user.Email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User registration failed. Email: {Email}", email);
            throw;
        }
    }
    
    
    private List<string> GetUserPermissions(AppUser user) 
    {
        var permissions = _context.UserRoles.Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.RolePermissions)
            .SelectMany(rp => rp)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();
        return permissions;
    }
    
    
}