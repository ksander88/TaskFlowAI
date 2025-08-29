// TaskFlowAI.Application/Abstractions/ITaskRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlowAI.Domain;

namespace TaskFlowAI.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem> AddAsync(TaskItem item);
    Task UpdateAsync(TaskItem item);
    Task DeleteAsync(int id);
    Task<TaskItem?> GetByIdAsync(int id);
    Task<IReadOnlyList<TaskItem>> GetAllAsync();
}