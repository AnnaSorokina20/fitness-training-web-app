using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.ViewModels.Admin;
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
            }).ToList()
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
    public async Task<IActionResult> RejectExercise(int id)
    {
        await moderationService.RejectExerciseAsync(id, User.GetUserId());
        return RedirectToAction(nameof(Index));
    }
}
