// TaskFlowAI.Infrastructure/Persistence/AppDbContext.cs
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using TaskFlowAI.Domain;

namespace TaskFlowAI.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    private readonly string _dbPath;
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public AppDbContext()
    {
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dir = Path.Combine(local, "TaskFlowAI");
        Directory.CreateDirectory(dir);
        _dbPath = Path.Combine(dir, "taskflow.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(300);
            e.Property(x => x.Description).HasDefaultValue(string.Empty);
            e.Property(x => x.Category).HasMaxLength(50).HasDefaultValue("Personal");
            e.Property(x => x.Priority).HasConversion<int>();
            e.Property(x => x.Status).HasConversion<int>();
        });
    }
}
