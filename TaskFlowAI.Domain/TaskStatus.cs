﻿// TaskFlowAI.Domain/TaskStatus.cs
namespace TaskFlowAI.Domain;

public enum TaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3
}