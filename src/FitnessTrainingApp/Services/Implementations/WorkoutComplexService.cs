using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class WorkoutComplexService(FitnessTrainingDbContext context) : IWorkoutComplexService
{
    public async Task<IReadOnlyList<WorkoutComplex>> GetAllAsync()
    {
        return await PublishedComplexes()
            .OrderBy(complex => complex.Name)
            .ToListAsync();
    }

    public async Task<WorkoutComplex?> GetDetailsAsync(int id)
    {
        return await PublishedComplexes()
            .Include(complex => complex.WorkoutComplexExercises.OrderBy(item => item.OrderNumber))
            .ThenInclude(item => item.Exercise)
            .FirstOrDefaultAsync(complex => complex.Id == id);
    }

    private IQueryable<WorkoutComplex> PublishedComplexes()
    {
        return context.WorkoutComplexes
            .AsNoTracking()
            .Where(complex => !complex.IsDeleted && complex.Status == ContentStatus.Published);
    }
}
