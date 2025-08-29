// TaskFlowAI.Presentation/TaskEditViewModel.cs
using System;
using TaskFlowAI.Domain;
using DomainTaskStatus = TaskFlowAI.Domain.TaskStatus;

namespace TaskFlowAI.Presentation;

public sealed class TaskEditViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "Personal";
    public Priority Priority { get; set; } = Priority.Medium;
    public DomainTaskStatus Status { get; set; } = DomainTaskStatus.Pending;
    public DateTime? DueDate { get; set; }
    public double? EstimatedHours { get; set; }

    public TaskItem ToEntity() => new()
    {
        Title = Title?.Trim() ?? "",
        Description = Description ?? "",
        Category = string.IsNullOrWhiteSpace(Category) ? "Personal" : Category.Trim(),
        Priority = Priority,
        Status = Status,
        DueDate = DueDate,
        EstimatedHours = EstimatedHours
    };

    public static TaskEditViewModel FromEntity(TaskItem t) => new()
    {
        Title = t.Title,
        Description = t.Description,
        Category = t.Category,
        Priority = t.Priority,
        Status = t.Status,
        DueDate = t.DueDate,
        EstimatedHours = t.EstimatedHours
    };
}
