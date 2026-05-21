using FitnessTrainingApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessTrainingApp.Data;

public sealed class FitnessTrainingDbContext(DbContextOptions<FitnessTrainingDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutComplex> WorkoutComplexes => Set<WorkoutComplex>();
    public DbSet<WorkoutComplexExercise> WorkoutComplexExercises => Set<WorkoutComplexExercise>();
    public DbSet<PlaylistItem> PlaylistItems => Set<PlaylistItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<AdminLog> AdminLogs => Set<AdminLog>();
    public DbSet<EntityChangeLog> EntityChangeLogs => Set<EntityChangeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.FullName).HasMaxLength(160).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.Property(exercise => exercise.Name).HasMaxLength(160).IsRequired();
            entity.Property(exercise => exercise.Description).HasMaxLength(2000).IsRequired();
            entity.Property(exercise => exercise.Equipment).HasMaxLength(160).IsRequired();
            entity.Property(exercise => exercise.MuscleGroup).HasMaxLength(120).IsRequired();
            entity.Property(exercise => exercise.SafetyNotes).HasMaxLength(1000);
            entity.Property(exercise => exercise.Difficulty).HasConversion<string>().HasMaxLength(32);
            entity.Property(exercise => exercise.WorkoutType).HasConversion<string>().HasMaxLength(32);
            entity.Property(exercise => exercise.Status).HasConversion<string>().HasMaxLength(32);
            entity.HasOne(exercise => exercise.Trainer)
                .WithMany(user => user.Exercises)
                .HasForeignKey(exercise => exercise.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkoutComplex>(entity =>
        {
            entity.Property(complex => complex.Name).HasMaxLength(160).IsRequired();
            entity.Property(complex => complex.Description).HasMaxLength(2000).IsRequired();
            entity.Property(complex => complex.Difficulty).HasConversion<string>().HasMaxLength(32);
            entity.Property(complex => complex.WorkoutType).HasConversion<string>().HasMaxLength(32);
            entity.Property(complex => complex.Status).HasConversion<string>().HasMaxLength(32);
            entity.HasOne(complex => complex.Trainer)
                .WithMany(user => user.WorkoutComplexes)
                .HasForeignKey(complex => complex.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WorkoutComplexExercise>(entity =>
        {
            entity.HasKey(item => new { item.WorkoutComplexId, item.ExerciseId });
            entity.HasOne(item => item.WorkoutComplex)
                .WithMany(complex => complex.WorkoutComplexExercises)
                .HasForeignKey(item => item.WorkoutComplexId);
            entity.HasOne(item => item.Exercise)
                .WithMany(exercise => exercise.WorkoutComplexExercises)
                .HasForeignKey(item => item.ExerciseId);
        });

        modelBuilder.Entity<PlaylistItem>(entity =>
        {
            entity.Property(item => item.ItemType).HasConversion<string>().HasMaxLength(32);
            entity.HasIndex(item => new { item.UserId, item.ExerciseId, item.WorkoutComplexId, item.ItemType }).IsUnique();
            entity.HasOne(item => item.User)
                .WithMany(user => user.PlaylistItems)
                .HasForeignKey(item => item.UserId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(comment => comment.Text).HasMaxLength(1000).IsRequired();
            entity.HasOne(comment => comment.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.UserId);
            entity.HasOne(comment => comment.Exercise)
                .WithMany(exercise => exercise.Comments)
                .HasForeignKey(comment => comment.ExerciseId);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasIndex(rating => new { rating.UserId, rating.ExerciseId }).IsUnique();
            entity.HasOne(rating => rating.User)
                .WithMany(user => user.Ratings)
                .HasForeignKey(rating => rating.UserId);
            entity.HasOne(rating => rating.Exercise)
                .WithMany(exercise => exercise.Ratings)
                .HasForeignKey(rating => rating.ExerciseId);
        });

        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.Property(file => file.FileName).HasMaxLength(255).IsRequired();
            entity.Property(file => file.Url).HasMaxLength(1000).IsRequired();
            entity.Property(file => file.ContentType).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.Property(session => session.Role).HasConversion<string>().HasMaxLength(32);
            entity.HasOne(session => session.User)
                .WithMany(user => user.Sessions)
                .HasForeignKey(session => session.UserId);
        });

        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.Property(log => log.Action).HasMaxLength(120).IsRequired();
            entity.Property(log => log.EntityName).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<EntityChangeLog>(entity =>
        {
            entity.Property(log => log.EntityName).HasMaxLength(120).IsRequired();
            entity.Property(log => log.Action).HasMaxLength(120).IsRequired();
        });
    }
}
