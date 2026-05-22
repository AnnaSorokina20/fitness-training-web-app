using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.ViewModels.Trainer;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public sealed class ExercisesController(IExerciseService exerciseService) : Controller
{
    [HttpGet]
    public IActionResult Create()
    {
        return View(new TrainerExerciseFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TrainerExerciseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var created = await exerciseService.CreatePublishedAsync(ToEntity(model, User.GetUserId()), model.MediaUrl);

        if (!created)
        {
            ModelState.AddModelError(string.Empty, "Exercise was not created. Check the form data.");
            return View(model);
        }

        return RedirectToAction("Index", "Moderation", new { area = "Admin" });
    }

    private static Exercise ToEntity(TrainerExerciseFormViewModel model, int adminId)
    {
        return new Exercise
        {
            Name = model.Name,
            Description = model.Description,
            Difficulty = model.Difficulty,
            WorkoutType = model.WorkoutType,
            Equipment = model.Equipment,
            MuscleGroup = model.MuscleGroup,
            SafetyNotes = model.SafetyNotes,
            TrainerId = adminId
        };
    }
}
