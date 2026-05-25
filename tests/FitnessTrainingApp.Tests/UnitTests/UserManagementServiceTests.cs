using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("UserManagementService")]
public sealed class UserManagementServiceTests
{
    [Test]
    public async Task GetAllAsync_ReturnsActiveUsersOrderedByEmail()
    {
        using var context = TestDbContextFactory.CreateContext();
        var second = TestDataFactory.CreateUser(2);
        second.Email = "z-user@test.local";
        var first = TestDataFactory.CreateUser(1);
        first.Email = "a-user@test.local";
        var deleted = TestDataFactory.CreateUser(3);
        deleted.IsDeleted = true;
        context.Users.AddRange(second, first, deleted);
        await context.SaveChangesAsync();

        var result = await new UserManagementService(context).GetAllAsync();

        Assert.That(result.Select(user => user.Email), Is.EqualTo(new[] { "a-user@test.local", "z-user@test.local" }));
    }

    [Test]
    public async Task ChangeRoleAsync_ValidUser_ChangesRoleAndCreatesAdminLog()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.AddRange(
            TestDataFactory.CreateUser(1, role: UserRole.User),
            TestDataFactory.CreateUser(2, role: UserRole.Administrator));
        await context.SaveChangesAsync();

        var result = await new UserManagementService(context).ChangeRoleAsync(1, UserRole.Trainer, 2);

        Assert.That(result, Is.True);
        Assert.That((await context.Users.FindAsync(1))!.Role, Is.EqualTo(UserRole.Trainer));
        Assert.That(await context.AdminLogs.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task ChangeRoleAsync_SelfRoleChange_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.Add(TestDataFactory.CreateUser(1, role: UserRole.Administrator));
        await context.SaveChangesAsync();

        var result = await new UserManagementService(context).ChangeRoleAsync(1, UserRole.User, 1);

        Assert.That(result, Is.False);
        Assert.That(await context.AdminLogs.CountAsync(), Is.Zero);
    }

    [Test]
    public async Task ChangeRoleAsync_SameRole_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.AddRange(
            TestDataFactory.CreateUser(1, role: UserRole.User),
            TestDataFactory.CreateUser(2, role: UserRole.Administrator));
        await context.SaveChangesAsync();

        var result = await new UserManagementService(context).ChangeRoleAsync(1, UserRole.User, 2);

        Assert.That(result, Is.False);
        Assert.That(await context.AdminLogs.CountAsync(), Is.Zero);
    }
}
