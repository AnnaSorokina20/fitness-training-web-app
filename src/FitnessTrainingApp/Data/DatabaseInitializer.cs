using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FitnessTrainingDbContext>();

        await context.Database.EnsureCreatedAsync();
    }
}
