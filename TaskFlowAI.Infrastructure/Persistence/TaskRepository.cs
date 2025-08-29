using Microsoft.EntityFrameworkCore;
using TaskFlowAI.Application.Interfaces;
using TaskFlowAI.Domain;

namespace TaskFlowAI.Infrastructure.Persistence;

public sealed class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db) => _db = db;

    public async Task<TaskItem> AddAsync(TaskItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        _db.Tasks.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task UpdateAsync(TaskItem item)
    {
        var existing = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == item.Id);
        if (existing == null) return;

        // Обновляем поля существующей сущности
        existing.Title = item.Title;
        existing.Description = item.Description;
        existing.Priority = item.Priority;
        existing.Status = item.Status;
        existing.Category = item.Category;
        existing.DueDate = item.DueDate;
        existing.EstimatedHours = item.EstimatedHours;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (entity != null)
        {
            _db.Tasks.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }

    public Task<TaskItem?> GetByIdAsync(int id)
        => _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync()
        => await _db.Tasks.AsNoTracking()
                          .OrderByDescending(t => t.CreatedAt)
                          .ToListAsync();
}
