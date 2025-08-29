// TaskFlowAI.Domain/TaskItem.cs
using System;

namespace TaskFlowAI.Domain;

public sealed class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;         // проверяем на пустоту при сохранении
    public string Description { get; set; } = string.Empty;   // многострочный ввод
    public Priority Priority { get; set; } = Priority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public string Category { get; set; } = "Personal";        // Work, Personal, Study
    public DateTime? DueDate { get; set; }
    public double? EstimatedHours { get; set; }               // оценка (ИИ/вручную)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}