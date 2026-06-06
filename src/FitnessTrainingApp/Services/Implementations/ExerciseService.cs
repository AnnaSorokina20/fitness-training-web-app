using FitnessTrainingApp.Data;
using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Services.Implementations;

public sealed class ExerciseService(FitnessTrainingDbContext context, IWebHostEnvironment environment) : IExerciseService
{
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedImageContentTypes = ["image/jpeg", "image/png", "image/webp"];
    private static readonly string[] AllowedHomeEquipment =
    [
        "no equipment",
        "none",
        "bodyweight",
        "resistance band",
        "dumbbell <= 10 kg",
        "dumbbell ≤ 10 кг"
    ];

    public async Task<IReadOnlyList<Exercise>> GetAllAsync()
    {
        return await PublishedExercises()
            .OrderBy(exercise => exercise.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Exercise>> SearchAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllAsync();
        }

        var normalizedKeyword = keyword.Trim().ToLower();

        return await PublishedExercises()
            .Where(exercise =>
                exercise.Name.ToLower().Contains(normalizedKeyword) ||
                exercise.MuscleGroup.ToLower().Contains(normalizedKeyword) ||
                exercise.Equipment.ToLower().Contains(normalizedKeyword))
            .OrderBy(exercise => exercise.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Exercise>> FilterAsync(
        DifficultyLevel? difficulty,
        string? equipment,
        string? muscleGroup,
        WorkoutType? workoutType)
    {
        var query = PublishedExercises();

        if (difficulty.HasValue)
        {
            query = query.Where(exercise => exercise.Difficulty == difficulty.Value);
        }

        if (!string.IsNullOrWhiteSpace(equipment))
        {
            var normalizedEquipment = equipment.Trim().ToLower();
            query = query.Where(exercise => exercise.Equipment.ToLower().Contains(normalizedEquipment));
        }

        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            var normalizedMuscleGroup = muscleGroup.Trim().ToLower();
            query = query.Where(exercise => exercise.MuscleGroup.ToLower().Contains(normalizedMuscleGroup));
        }

        if (workoutType.HasValue)
        {
            query = query.Where(exercise => exercise.WorkoutType == workoutType.Value);
        }

        return await query
            .OrderBy(exercise => exercise.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetSuggestionsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var normalizedQuery = query.Trim().ToLower();

        var exercises = await PublishedExercises()
            .Where(exercise =>
                exercise.Name.ToLower().Contains(normalizedQuery) ||
                exercise.MuscleGroup.ToLower().Contains(normalizedQuery) ||
                exercise.Equipment.ToLower().Contains(normalizedQuery))
            .OrderBy(exercise => exercise.Name)
            .Select(exercise => new
            {
                exercise.Name,
                exercise.MuscleGroup,
                exercise.Equipment
            })
            .Take(12)
            .ToListAsync();

        return exercises
            .SelectMany(exercise => new[] { exercise.Name, exercise.MuscleGroup, exercise.Equipment })
            .Where(value => value.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToList();
    }

    public async Task<Exercise?> GetDetailsAsync(int id)
    {
        return await PublishedExercises()
            .Include(exercise => exercise.Trainer)
            .Include(exercise => exercise.MediaFiles)
            .Include(exercise => exercise.Comments)
            .Include(exercise => exercise.Ratings)
            .FirstOrDefaultAsync(exercise => exercise.Id == id);
    }

    public async Task<IReadOnlyList<Exercise>> GetForTrainerAsync(int trainerId)
    {
        return await context.Exercises
            .AsNoTracking()
            .Where(exercise => exercise.TrainerId == trainerId && !exercise.IsDeleted)
            .OrderByDescending(exercise => exercise.UpdatedAt ?? exercise.CreatedAt)
            .ToListAsync();
    }

    public async Task<Exercise?> GetTrainerExerciseAsync(int id, int trainerId)
    {
        return await context.Exercises
            .AsNoTracking()
            .Include(exercise => exercise.MediaFiles)
            .FirstOrDefaultAsync(exercise => exercise.Id == id && exercise.TrainerId == trainerId && !exercise.IsDeleted);
    }

    public async Task<bool> CreateForTrainerAsync(Exercise exercise, string? mediaUrls, IReadOnlyList<IFormFile>? uploadedImages)
    {
        return await CreateAsync(exercise, mediaUrls, uploadedImages, ContentStatus.PendingModeration);
    }

    public async Task<bool> CreatePublishedAsync(Exercise exercise, string? mediaUrls, IReadOnlyList<IFormFile>? uploadedImages)
    {
        return await CreateAsync(exercise, mediaUrls, uploadedImages, ContentStatus.Published);
    }

    private async Task<bool> CreateAsync(Exercise exercise, string? mediaUrls, IReadOnlyList<IFormFile>? uploadedImages, ContentStatus status)
    {
        var mediaFiles = await BuildMediaFilesAsync(mediaUrls, uploadedImages);

        if (!IsValidTrainerExercise(exercise) || mediaFiles.Count == 0)
        {
            return false;
        }

        exercise.Status = status;
        exercise.ModerationComment = null;
        exercise.CreatedAt = DateTime.UtcNow;

        context.Exercises.Add(exercise);
        await context.SaveChangesAsync();

        foreach (var mediaFile in mediaFiles)
        {
            mediaFile.ExerciseId = exercise.Id;
            context.MediaFiles.Add(mediaFile);
        }

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateForTrainerAsync(int id, int trainerId, Exercise exercise, string? mediaUrls, IReadOnlyList<IFormFile>? uploadedImages)
    {
        var mediaFiles = await BuildMediaFilesAsync(mediaUrls, uploadedImages);

        if (!IsValidTrainerExercise(exercise) || mediaFiles.Count == 0)
        {
            return false;
        }

        var existing = await context.Exercises
            .Include(item => item.MediaFiles)
            .FirstOrDefaultAsync(item => item.Id == id && item.TrainerId == trainerId && !item.IsDeleted);

        if (existing is null)
        {
            return false;
        }

        existing.Name = exercise.Name.Trim();
        existing.Description = exercise.Description.Trim();
        existing.Difficulty = exercise.Difficulty;
        existing.WorkoutType = exercise.WorkoutType;
        existing.Equipment = exercise.Equipment.Trim();
        existing.MuscleGroup = exercise.MuscleGroup.Trim();
        existing.SafetyNotes = exercise.SafetyNotes.Trim();
        existing.Status = ContentStatus.PendingModeration;
        existing.ModerationComment = null;
        existing.UpdatedAt = DateTime.UtcNow;

        foreach (var mediaFile in existing.MediaFiles)
        {
            DeleteLocalMediaFile(mediaFile.Url);
        }

        context.MediaFiles.RemoveRange(existing.MediaFiles);

        foreach (var mediaFile in mediaFiles)
        {
            mediaFile.ExerciseId = existing.Id;
            context.MediaFiles.Add(mediaFile);
        }

        await context.SaveChangesAsync();
        return true;
    }

    private IQueryable<Exercise> PublishedExercises()
    {
        return context.Exercises
            .AsNoTracking()
            .Include(exercise => exercise.MediaFiles)
            .Where(exercise => !exercise.IsDeleted && exercise.Status == ContentStatus.Published);
    }

    private static bool IsValidTrainerExercise(Exercise exercise)
    {
        return !string.IsNullOrWhiteSpace(exercise.Name) &&
               !string.IsNullOrWhiteSpace(exercise.Description) &&
               !string.IsNullOrWhiteSpace(exercise.Equipment) &&
               !string.IsNullOrWhiteSpace(exercise.MuscleGroup) &&
               !string.IsNullOrWhiteSpace(exercise.SafetyNotes) &&
               IsAllowedEquipmentForWorkoutType(exercise.WorkoutType, exercise.Equipment);
    }

    private static bool IsAllowedEquipmentForWorkoutType(WorkoutType workoutType, string equipment)
    {
        if (workoutType != WorkoutType.Home)
        {
            return true;
        }

        var normalizedEquipment = equipment.Trim().ToLowerInvariant();

        return AllowedHomeEquipment.Contains(normalizedEquipment);
    }

    private async Task<IReadOnlyList<MediaFile>> BuildMediaFilesAsync(string? mediaUrls, IReadOnlyList<IFormFile>? uploadedImages)
    {
        var mediaFiles = new List<MediaFile>();
        var images = (uploadedImages ?? [])
            .Where(file => file.Length > 0)
            .ToList();

        if (images.Any(file => !IsValidUploadedImage(file)))
        {
            return [];
        }

        foreach (var mediaUrl in ParseMediaUrls(mediaUrls))
        {
            mediaFiles.Add(CreateExternalMediaFile(mediaUrl));
        }

        foreach (var uploadedImage in images)
        {
            mediaFiles.Add(await SaveUploadedImageAsync(uploadedImage));
        }

        return mediaFiles;
    }

    private static IReadOnlyList<string> ParseMediaUrls(string? mediaUrls)
    {
        if (string.IsNullOrWhiteSpace(mediaUrls))
        {
            return [];
        }

        return mediaUrls
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
            .Select(url => url.Trim())
            .Where(url => Uri.TryCreate(url, UriKind.Absolute, out var parsedUrl) && parsedUrl.Scheme is "http" or "https")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static MediaFile CreateExternalMediaFile(string mediaUrl)
    {
        return new MediaFile
        {
            Url = mediaUrl,
            FileName = Path.GetFileName(new Uri(mediaUrl).AbsolutePath),
            ContentType = InferContentType(mediaUrl)
        };
    }

    private static bool IsValidUploadedImage(IFormFile uploadedImage)
    {
        if (!AllowedImageContentTypes.Contains(uploadedImage.ContentType) || uploadedImage.Length > MaxImageSizeBytes)
        {
            return false;
        }

        var extension = Path.GetExtension(uploadedImage.FileName).ToLowerInvariant();

        return extension is ".jpg" or ".jpeg" or ".png" or ".webp";
    }

    private async Task<MediaFile> SaveUploadedImageAsync(IFormFile uploadedImage)
    {
        var extension = Path.GetExtension(uploadedImage.FileName).ToLowerInvariant();
        var uploadsDirectory = Path.Combine(environment.WebRootPath, "uploads", "exercises");
        Directory.CreateDirectory(uploadsDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsDirectory, fileName);

        await using var stream = File.Create(filePath);
        await uploadedImage.CopyToAsync(stream);

        return new MediaFile
        {
            Url = $"/uploads/exercises/{fileName}",
            FileName = fileName,
            ContentType = uploadedImage.ContentType
        };
    }

    private void DeleteLocalMediaFile(string url)
    {
        if (!url.StartsWith("/uploads/exercises/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fileName = Path.GetFileName(url);
        var filePath = Path.Combine(environment.WebRootPath, "uploads", "exercises", fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private static string InferContentType(string mediaUrl)
    {
        var lowerUrl = mediaUrl.ToLowerInvariant();

        if (lowerUrl.Contains("youtube.com/watch") || lowerUrl.Contains("youtu.be/"))
        {
            return "video/youtube";
        }

        return Path.GetExtension(new Uri(mediaUrl).AbsolutePath).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            _ => "text/uri-list"
        };
    }
}
