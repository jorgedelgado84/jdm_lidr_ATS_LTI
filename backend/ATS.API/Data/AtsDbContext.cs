using ATS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ATS.API.Data;

/// <summary>
/// Entity Framework Core database context for the ATS application
/// Configures entity relationships, mappings, and database constraints
/// Demonstrates Single Responsibility Principle (configuration only)
/// </summary>
public class AtsDbContext : DbContext
{
    public AtsDbContext(DbContextOptions<AtsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<Candidate> Candidates { get; set; } = null!;
    public DbSet<Application> Applications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedPositions)
            .WithOne(p => p.CreatedByUser)
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.ReviewedApplications)
            .WithOne(a => a.ReviewedByUser)
            .HasForeignKey(a => a.ReviewedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Position configuration
        modelBuilder.Entity<Position>()
            .HasIndex(p => p.Status);

        modelBuilder.Entity<Position>()
            .HasMany(p => p.Applications)
            .WithOne(a => a.Position)
            .HasForeignKey(a => a.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Candidate configuration
        modelBuilder.Entity<Candidate>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Candidate>()
            .HasMany(c => c.Applications)
            .WithOne(a => a.Candidate)
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Application configuration
        modelBuilder.Entity<Application>()
            .HasIndex(a => new { a.CandidateId, a.PositionId })
            .IsUnique();

        modelBuilder.Entity<Application>()
            .HasIndex(a => a.Status);

        modelBuilder.Entity<Application>()
            .HasIndex(a => a.SubmittedAt);
    }
}
