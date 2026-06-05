using FitnessTrainingApp.Models.Entities;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("AdminLogService")]
public sealed class AdminLogServiceTests
{
    [Test]
    public async Task GetAllAsync_ReturnsLogsNewestFirstWithAdmin()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.Add(TestDataFactory.CreateUser(1, role: UserRole.Administrator));
        context.AdminLogs.AddRange(
            new AdminLog
            {
                AdminId = 1,
                Action = "OlderAction",
                EntityName = "User",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new AdminLog
            {
                AdminId = 1,
                Action = "NewerAction",
                EntityName = "User",
                CreatedAt = DateTime.UtcNow
            });
        await context.SaveChangesAsync();

        var result = await new AdminLogService(context).GetAllAsync();

        Assert.That(result.First().Action, Is.EqualTo("NewerAction"));
        Assert.That(result.First().Admin?.Role, Is.EqualTo(UserRole.Administrator));
    }
}
