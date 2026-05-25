using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FitnessTrainingDbContext>();

        await context.Database.EnsureCreatedAsync();
        await ApplySchemaUpdatesAsync(context);
        await SeedMissingDataAsync(context);
    }

    private static async Task ApplySchemaUpdatesAsync(FitnessTrainingDbContext context)
    {
        if (!context.Database.IsNpgsql())
        {
            return;
        }

        await context.Database.ExecuteSqlRawAsync("""ALTER TABLE "Exercises" ADD COLUMN IF NOT EXISTS "ModerationComment" character varying(1000);""");
        await context.Database.ExecuteSqlRawAsync("""ALTER TABLE "WorkoutComplexes" ADD COLUMN IF NOT EXISTS "ModerationComment" character varying(1000);""");
        await context.Database.ExecuteSqlRawAsync("""ALTER TABLE "Comments" ADD COLUMN IF NOT EXISTS "WorkoutComplexId" integer;""");
        await context.Database.ExecuteSqlRawAsync("""ALTER TABLE "Comments" ALTER COLUMN "ExerciseId" DROP NOT NULL;""");
        await context.Database.ExecuteSqlRawAsync("""ALTER TABLE "Ratings" ADD COLUMN IF NOT EXISTS "WorkoutComplexId" integer;""");
        await context.Database.ExecuteSqlRawAsync("""ALTER TABLE "Ratings" ALTER COLUMN "ExerciseId" DROP NOT NULL;""");
        await context.Database.ExecuteSqlRawAsync("""CREATE UNIQUE INDEX IF NOT EXISTS "IX_Ratings_UserId_WorkoutComplexId" ON "Ratings" ("UserId", "WorkoutComplexId");""");
    }

    private static async Task SeedMissingDataAsync(FitnessTrainingDbContext context)
    {
        foreach (var user in SeedData.Users)
        {
            if (!await context.Users.AnyAsync(existing => existing.Id == user.Id))
            {
                context.Users.Add(user);
            }
            else
            {
                var existingUser = await context.Users.FirstAsync(existing => existing.Id == user.Id);

                if (existingUser.PasswordHash.StartsWith("seed-", StringComparison.Ordinal))
                {
                    existingUser.PasswordHash = user.PasswordHash;
                }
            }
        }

        foreach (var exercise in SeedData.Exercises)
        {
            if (!await context.Exercises.AnyAsync(existing => existing.Id == exercise.Id))
            {
                context.Exercises.Add(exercise);
            }
        }

        foreach (var mediaFile in SeedData.MediaFiles)
        {
            if (!await context.MediaFiles.AnyAsync(existing => existing.Id == mediaFile.Id))
            {
                context.MediaFiles.Add(mediaFile);
            }
        }

        foreach (var workoutComplex in SeedData.WorkoutComplexes)
        {
            if (!await context.WorkoutComplexes.AnyAsync(existing => existing.Id == workoutComplex.Id))
            {
                context.WorkoutComplexes.Add(workoutComplex);
            }
        }

        foreach (var item in SeedData.WorkoutComplexExercises)
        {
            var exists = await context.WorkoutComplexExercises.AnyAsync(existing =>
                existing.WorkoutComplexId == item.WorkoutComplexId &&
                existing.ExerciseId == item.ExerciseId);

            if (!exists)
            {
                context.WorkoutComplexExercises.Add(item);
            }
        }

        await context.SaveChangesAsync();
        await ResetPostgresSequencesAsync(context);
    }

    private static async Task ResetPostgresSequencesAsync(FitnessTrainingDbContext context)
    {
        if (!context.Database.IsNpgsql())
        {
            return;
        }

        await ResetPostgresSequenceAsync(context, "Users");
        await ResetPostgresSequenceAsync(context, "Exercises");
        await ResetPostgresSequenceAsync(context, "MediaFiles");
        await ResetPostgresSequenceAsync(context, "WorkoutComplexes");
    }

    private static async Task ResetPostgresSequenceAsync(FitnessTrainingDbContext context, string tableName)
    {
        var sql = tableName switch
        {
            "Users" => """SELECT setval(pg_get_serial_sequence('"public"."Users"', 'Id'), COALESCE((SELECT MAX("Id") FROM "public"."Users"), 1), (SELECT COUNT(*) > 0 FROM "public"."Users"));""",
            "Exercises" => """SELECT setval(pg_get_serial_sequence('"public"."Exercises"', 'Id'), COALESCE((SELECT MAX("Id") FROM "public"."Exercises"), 1), (SELECT COUNT(*) > 0 FROM "public"."Exercises"));""",
            "MediaFiles" => """SELECT setval(pg_get_serial_sequence('"public"."MediaFiles"', 'Id'), COALESCE((SELECT MAX("Id") FROM "public"."MediaFiles"), 1), (SELECT COUNT(*) > 0 FROM "public"."MediaFiles"));""",
            "WorkoutComplexes" => """SELECT setval(pg_get_serial_sequence('"public"."WorkoutComplexes"', 'Id'), COALESCE((SELECT MAX("Id") FROM "public"."WorkoutComplexes"), 1), (SELECT COUNT(*) > 0 FROM "public"."WorkoutComplexes"));""",
            _ => throw new InvalidOperationException("Unknown seeded table.")
        };

        await context.Database.ExecuteSqlRawAsync(sql);
    }
}
