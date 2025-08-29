// TaskFlowAI.Application/Interfaces/IReminderScheduler.cs
using System;
using System.Threading.Tasks;

namespace TaskFlowAI.Application.Interfaces;

public interface IReminderScheduler
{
    /// <summary>
    /// Запланировать напоминание (асинхронно).
    /// </summary>
    Task ScheduleReminderAsync(string message, DateTime time);

    /// <summary>
    /// Запустить планировщик (если нужен цикл обслуживания).
    /// </summary>
    void Start();

    /// <summary>
    /// Остановить планировщик и отменить все запланированные задачи.
    /// </summary>
    void Stop();
}
