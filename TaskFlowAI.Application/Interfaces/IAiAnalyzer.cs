// TaskFlowAI.Application/Abstractions/IAiAnalyzer.cs
using TaskFlowAI.Domain;

namespace TaskFlowAI.Application.Interfaces;

public interface IAiAnalyzer
{
    public (Priority suggestedPriority, double? suggestedHours, string explanation) Analyze(TaskItem item);
}