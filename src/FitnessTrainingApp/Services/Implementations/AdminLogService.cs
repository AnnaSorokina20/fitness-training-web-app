using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class AdminLogService(FitnessTrainingDbContext context) : IAdminLogService
{
    public async Task<IReadOnlyList<AdminLog>> GetAllAsync()
    {
        return await context.AdminLogs
            .AsNoTracking()
            .Include(log => log.Admin)
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync();
    }
}
