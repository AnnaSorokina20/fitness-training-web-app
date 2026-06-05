using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class UserManagementService(FitnessTrainingDbContext context) : IUserManagementService
{
    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await context.Users
            .AsNoTracking()
            .Where(user => !user.IsDeleted)
            .OrderBy(user => user.Email)
            .ToListAsync();
    }

    public async Task<bool> ChangeRoleAsync(int userId, UserRole role, int adminId)
    {
        if (userId == adminId)
        {
            return false;
        }

        var user = await context.Users.FirstOrDefaultAsync(existing => existing.Id == userId && !existing.IsDeleted);

        if (user is null || user.Role == role)
        {
            return false;
        }

        user.Role = role;
        user.UpdatedAt = DateTime.UtcNow;
        context.AdminLogs.Add(new AdminLog
        {
            AdminId = adminId,
            Action = "ChangeUserRole",
            EntityName = nameof(User),
            EntityId = user.Id,
            TargetUserId = user.Id
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int userId, int adminId)
    {
        if (userId == adminId)
        {
            return false;
        }

        var user = await context.Users.FirstOrDefaultAsync(existing => existing.Id == userId && !existing.IsDeleted);

        if (user is null)
        {
            return false;
        }

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        context.AdminLogs.Add(new AdminLog
        {
            AdminId = adminId,
            Action = "DeleteUser",
            EntityName = nameof(User),
            EntityId = user.Id,
            TargetUserId = user.Id
        });

        await context.SaveChangesAsync();
        return true;
    }
}
