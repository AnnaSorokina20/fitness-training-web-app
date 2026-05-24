using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.ViewModels.Shared;
using FitnessTrainingApp.Models.ViewModels.WorkoutComplexes;
using FitnessTrainingApp.Infrastructure.Extensions;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrainingApp.Controllers;

public sealed class WorkoutComplexesController(
    IWorkoutComplexService workoutComplexService,
    IPlaylistService playlistService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
    {
        var complexes = await workoutComplexService.GetAllAsync();
        var cards = complexes.Select(ToCardViewModel).ToList();

        return View(new WorkoutComplexCatalogViewModel
        {
            WorkoutComplexes = CreatePagedList(cards, page, pageSize)
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var complex = await workoutComplexService.GetDetailsAsync(id);

        if (complex is null)
        {
            return NotFound();
        }

        var userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : 0;

        return View(new WorkoutComplexDetailsViewModel
        {
            Id = complex.Id,
            Name = complex.Name,
            Description = complex.Description,
            Difficulty = complex.Difficulty,
            WorkoutType = complex.WorkoutType,
            DurationMinutes = complex.DurationMinutes,
            PlaylistItemId = userId == 0 ? null : await playlistService.GetWorkoutComplexPlaylistItemIdAsync(userId, id),
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
                .ToList()
        });
    }

    private static WorkoutComplexCardViewModel ToCardViewModel(WorkoutComplex complex)
    {
        return new WorkoutComplexCardViewModel
        {
            Id = complex.Id,
            Name = complex.Name,
            Description = complex.Description,
            Difficulty = complex.Difficulty,
            WorkoutType = complex.WorkoutType,
            DurationMinutes = complex.DurationMinutes,
            ExerciseCount = complex.WorkoutComplexExercises.Count
        };
    }

    private static PagedListViewModel<T> CreatePagedList<T>(IReadOnlyList<T> items, int page, int pageSize)
    {
        var normalizedPageSize = pageSize is 24 or 48 ? pageSize : 12;
        var totalPages = Math.Max(1, (int)Math.Ceiling(items.Count / (double)normalizedPageSize));
        var normalizedPage = Math.Clamp(page, 1, totalPages);

        return new PagedListViewModel<T>
        {
            Items = items
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize)
                .ToList(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = normalizedPage,
                PageSize = normalizedPageSize,
                TotalItems = items.Count,
                ControllerName = "WorkoutComplexes"
            }
        };
    }
}
