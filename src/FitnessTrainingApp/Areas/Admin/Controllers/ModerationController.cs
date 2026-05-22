using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.ViewModels.Admin;
using FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public sealed class ModerationController(IModerationService moderationService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var pendingExercises = await moderationService.GetPendingExercisesAsync();
        var pendingWorkoutComplexes = await moderationService.GetPendingWorkoutComplexesAsync();

        return View(new ModerationIndexViewModel
        {
            PendingExercises = pendingExercises.Select(exercise => new PendingExerciseViewModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Description = exercise.Description,
                TrainerName = exercise.Trainer?.FullName ?? "Trainer",
                Difficulty = exercise.Difficulty,
                WorkoutType = exercise.WorkoutType,
                MuscleGroup = exercise.MuscleGroup,
                Equipment = exercise.Equipment,
                SubmittedAt = exercise.UpdatedAt ?? exercise.CreatedAt
            }).ToList(),
            PendingWorkoutComplexes = pendingWorkoutComplexes.Select(complex => new PendingWorkoutComplexViewModel
            {
                Id = complex.Id,
                Name = complex.Name,
                Description = complex.Description,
                TrainerName = complex.Trainer?.FullName ?? "Trainer",
                Difficulty = complex.Difficulty,
                WorkoutType = complex.WorkoutType,
                DurationMinutes = complex.DurationMinutes,
                ExerciseCount = complex.WorkoutComplexExercises.Count,
                SubmittedAt = complex.UpdatedAt ?? complex.CreatedAt
            }).ToList()
        });
    }

    public async Task<IActionResult> ExerciseDetails(int id)
    {
        var exercise = await moderationService.GetPendingExerciseDetailsAsync(id);

        if (exercise is null)
        {
            return NotFound();
        }

        return View(new PendingExerciseViewModel
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            TrainerName = exercise.Trainer?.FullName ?? "Trainer",
            Difficulty = exercise.Difficulty,
            WorkoutType = exercise.WorkoutType,
            MuscleGroup = exercise.MuscleGroup,
            Equipment = exercise.Equipment,
            SafetyNotes = exercise.SafetyNotes,
            MediaUrls = exercise.MediaFiles.Select(file => file.Url).ToList(),
            SubmittedAt = exercise.UpdatedAt ?? exercise.CreatedAt
        });
    }

    public async Task<IActionResult> WorkoutComplexDetails(int id)
    {
        var complex = await moderationService.GetPendingWorkoutComplexDetailsAsync(id);

        if (complex is null)
        {
            return NotFound();
        }

        return View(new PendingWorkoutComplexViewModel
        {
            Id = complex.Id,
            Name = complex.Name,
            Description = complex.Description,
            TrainerName = complex.Trainer?.FullName ?? "Trainer",
            Difficulty = complex.Difficulty,
            WorkoutType = complex.WorkoutType,
            DurationMinutes = complex.DurationMinutes,
            ExerciseCount = complex.WorkoutComplexExercises.Count,
            Exercises = complex.WorkoutComplexExercises
                .OrderBy(item => item.OrderNumber)
                .Select(item => new WorkoutComplexExerciseViewModel
                {
                    ExerciseId = item.ExerciseId,
                    ExerciseName = item.Exercise?.Name ?? "Exercise",
                    MuscleGroup = item.Exercise?.MuscleGroup ?? string.Empty,
                    Equipment = item.Exercise?.Equipment ?? string.Empty,
                    OrderNumber = item.OrderNumber,
                    Sets = item.Sets,
                    Repetitions = item.Repetitions
                })
                .ToList(),
            SubmittedAt = complex.UpdatedAt ?? complex.CreatedAt
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublishExercise(int id)
    {
        await moderationService.PublishExerciseAsync(id, User.GetUserId());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectExercise(int id, string? moderationComment)
    {
        await moderationService.RejectExerciseAsync(id, User.GetUserId(), moderationComment);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublishWorkoutComplex(int id)
    {
        await moderationService.PublishWorkoutComplexAsync(id, User.GetUserId());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectWorkoutComplex(int id, string? moderationComment)
    {
        await moderationService.RejectWorkoutComplexAsync(id, User.GetUserId(), moderationComment);
        return RedirectToAction(nameof(Index));
    }
}
