using FitnessTrainingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Tests.Support;

public static class TestDbContextFactory
{
    public static FitnessTrainingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<FitnessTrainingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FitnessTrainingDbContext(options);
    }
}
