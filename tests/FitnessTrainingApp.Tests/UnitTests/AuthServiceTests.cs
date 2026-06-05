using FitnessTrainingApp.Infrastructure.Security;
using FitnessTrainingApp.Models.Entities.Enums;
using FitnessTrainingApp.Services.Implementations;
using FitnessTrainingApp.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Tests.UnitTests;

[TestFixture]
[Category("Unit")]
[Category("AuthService")]
public sealed class AuthServiceTests
{
    [Test]
    public async Task RegisterAsync_ValidData_CreatesUserWithUserRole()
    {
        using var context = TestDbContextFactory.CreateContext();

        var result = await new AuthService(context).RegisterAsync("Anna User", "anna@test.local", "Password1");

        var user = await context.Users.SingleAsync();
        Assert.That(result, Is.True);
        Assert.That(user.Role, Is.EqualTo(UserRole.User));
        Assert.That(user.FullName, Is.EqualTo("Anna User"));
    }

    [Test]
    public async Task RegisterAsync_StoresPasswordHash()
    {
        using var context = TestDbContextFactory.CreateContext();

        await new AuthService(context).RegisterAsync("Anna User", "anna@test.local", "Password1");

        var user = await context.Users.SingleAsync();
        Assert.That(user.PasswordHash, Is.Not.EqualTo("Password1"));
        Assert.That(PasswordHasher.VerifyPassword("Password1", user.PasswordHash), Is.True);
    }

    [Test]
    public async Task RegisterAsync_DuplicateEmail_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new AuthService(context);

        await service.RegisterAsync("Anna User", "anna@test.local", "Password1");
        var duplicateResult = await service.RegisterAsync("Second User", "ANNA@test.local", "Password1");

        Assert.That(duplicateResult, Is.False);
        Assert.That(await context.Users.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task RegisterAsync_InvalidPassword_ReturnsFalse()
    {
        using var context = TestDbContextFactory.CreateContext();

        var result = await new AuthService(context).RegisterAsync("Anna User", "anna@test.local", "short");

        Assert.That(result, Is.False);
        Assert.That(await context.Users.CountAsync(), Is.Zero);
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsUser()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.Add(TestDataFactory.CreateUser(password: "Password1"));
        await context.SaveChangesAsync();

        var result = await new AuthService(context).LoginAsync("USER1@test.local", "Password1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Email, Is.EqualTo("user1@test.local"));
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        using var context = TestDbContextFactory.CreateContext();
        context.Users.Add(TestDataFactory.CreateUser(password: "Password1"));
        await context.SaveChangesAsync();

        var result = await new AuthService(context).LoginAsync("user1@test.local", "WrongPassword1");

        Assert.That(result, Is.Null);
    }
}
