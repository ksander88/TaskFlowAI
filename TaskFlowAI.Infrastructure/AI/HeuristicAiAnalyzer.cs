using TaskFlowAI.Domain;
using TaskFlowAI.Application.Interfaces;

namespace TaskFlowAI.Infrastructure.AI
{
    public class HeuristicAiAnalyzer : IAiAnalyzer
    {
        public (Priority suggestedPriority, double? suggestedHours, string explanation) Analyze(TaskItem task)
        {
            if (task == null)
            {
                return (Priority.Low, null, "Задача отсутствует");
            }

            Priority suggestedPriority;
            double? suggestedHours = null;
            string explanation;

            if (task.Priority == Priority.Urgent ||
                (task.DueDate.HasValue && task.DueDate.Value < DateTime.Now))
            {
                suggestedPriority = Priority.Urgent;
                explanation = "Высокий риск просрочки! Срочно выполнить.";
                suggestedHours = 1.0; // пример оценки времени
            }
            else if (task.Priority == Priority.High)
            {
                suggestedPriority = Priority.High;
                explanation = "Задачу рекомендуется выполнить в первую очередь.";
                suggestedHours = 2.0;
            }
            else if (task.Priority == Priority.Medium)
            {
                suggestedPriority = Priority.Medium;
                explanation = "Можно выполнить после более приоритетных задач.";
                suggestedHours = 3.0;
            }
            else
            {
                suggestedPriority = Priority.Low;
                explanation = "Низкий приоритет. Выполнить при наличии времени.";
                suggestedHours = 4.0;
            }

            return (suggestedPriority, suggestedHours, explanation);
        }
    }
}