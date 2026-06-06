using FitnessTrainingApp.Data;
using FitnessTrainingApp.Infrastructure.Security;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class AuthService(FitnessTrainingDbContext context) : IAuthService
{
    public async Task<User?> LoginAsync(string email, string password)
    {
        var normalizedEmail = NormalizeEmail(email);
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(existing => existing.Email.ToLower() == normalizedEmail && !existing.IsDeleted);

        if (user is null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        return user;
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == userId && !user.IsDeleted);
    }

    public async Task<bool> RegisterAsync(string fullName, string email, string password)
    {
        if (!IsValidEmail(email) || !IsValidPassword(password) || string.IsNullOrWhiteSpace(fullName))
        {
            return false;
        }

        if (await EmailExistsAsync(email))
        {
            return false;
        }

        context.Users.Add(new User
        {
            FullName = fullName.Trim(),
            Email = email.Trim(),
            PasswordHash = PasswordHasher.HashPassword(password),
            Role = UserRole.User
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProfileAsync(int userId, string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName) || !IsValidEmail(email))
        {
            return false;
        }

        var normalizedEmail = NormalizeEmail(email);
        var emailTaken = await context.Users.AnyAsync(user =>
            user.Id != userId &&
            user.Email.ToLower() == normalizedEmail &&
            !user.IsDeleted);

        if (emailTaken)
        {
            return false;
        }

        var user = await context.Users.FirstOrDefaultAsync(existing => existing.Id == userId && !existing.IsDeleted);

        if (user is null)
        {
            return false;
        }

        user.FullName = fullName.Trim();
        user.Email = email.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        if (!IsValidPassword(newPassword))
        {
            return false;
        }

        var user = await context.Users.FirstOrDefaultAsync(existing => existing.Id == userId && !existing.IsDeleted);

        if (user is null || !PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = PasswordHasher.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        return await context.Users.AnyAsync(user => user.Email.ToLower() == normalizedEmail);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLower();
    }

    private static bool IsValidEmail(string email)
    {
        return email.Contains('@') && email.Contains('.');
    }

    private static bool IsValidPassword(string password)
    {
        return password.Length >= 8 && password.Any(char.IsDigit);
    }
}
