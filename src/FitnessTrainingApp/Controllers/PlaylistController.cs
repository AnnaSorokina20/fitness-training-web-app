using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Models.ViewModels.Playlist;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Controllers;

[Authorize]
public sealed class PlaylistController(IPlaylistService playlistService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        var items = await playlistService.GetPlaylistAsync(userId);

        return View(new PlaylistIndexViewModel
        {
            Items = items.Select(ToViewModel).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExercise(int exerciseId, string? returnUrl = null)
    {
        await playlistService.AddExerciseAsync(User.GetUserId(), exerciseId);
        return RedirectToSafeReturnUrl(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddWorkoutComplex(int workoutComplexId, string? returnUrl = null)
    {
        await playlistService.AddWorkoutComplexAsync(User.GetUserId(), workoutComplexId);
        return RedirectToSafeReturnUrl(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id, string? returnUrl = null)
    {
        await playlistService.RemoveAsync(User.GetUserId(), id);
        return RedirectToSafeReturnUrl(returnUrl);
    }

    private IActionResult RedirectToSafeReturnUrl(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    private static PlaylistItemViewModel ToViewModel(PlaylistItem item)
    {
        if (item.ItemType == PlaylistItemType.Exercise && item.Exercise is not null)
        {
            return new PlaylistItemViewModel
            {
                Id = item.Id,
                ItemId = item.Exercise.Id,
                ItemType = item.ItemType,
                Title = item.Exercise.Name,
                Description = item.Exercise.Description,
                Meta = $"{item.Exercise.WorkoutType} · {item.Exercise.Difficulty} · {item.Exercise.MuscleGroup}"
            };
        }

        var complex = item.WorkoutComplex;

        return new PlaylistItemViewModel
        {
            Id = item.Id,
            ItemId = complex?.Id ?? 0,
            ItemType = item.ItemType,
            Title = complex?.Name ?? "Workout complex",
            Description = complex?.Description ?? string.Empty,
            Meta = complex is null ? string.Empty : $"{complex.WorkoutType} · {complex.Difficulty} · {complex.DurationMinutes} min"
        };
    }
}
