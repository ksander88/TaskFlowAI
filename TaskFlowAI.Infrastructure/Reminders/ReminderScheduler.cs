// TaskFlowAI.Infrastructure/Reminders/ReminderScheduler.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskFlowAI.Application.Interfaces;

namespace TaskFlowAI.Infrastructure.Reminders;

public sealed class ReminderScheduler : IReminderScheduler
{
    private readonly List<Task> _reminders = new();
    private CancellationTokenSource? _cts;
    private readonly object _sync = new();

    public Task ScheduleReminderAsync(string message, DateTime time)
    {
        lock (_sync)
        {
            if (_cts == null)
                _cts = new CancellationTokenSource();
        }

        var delay = time - DateTime.Now;
        if (delay.TotalMilliseconds <= 0) return Task.CompletedTask;

        var token = _cts.Token;
        var t = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, token);
                if (!token.IsCancellationRequested)
                    Console.WriteLine($"🔔 Reminder: {message}");
            }
            catch (TaskCanceledException) { }
        }, token);

        lock (_sync) { _reminders.Add(t); }
        return t;
    }

    public void Start()
    {
        lock (_sync)
        {
            if (_cts == null || _cts.IsCancellationRequested)
                _cts = new CancellationTokenSource();
        }
    }

    public void Stop()
    {
        lock (_sync)
        {
            try
            {
                _cts?.Cancel();
            }
            catch { }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }

            // Пытаемся дождаться фоновых задач (не блокируя UI)
            if (_reminders.Count > 0)
            {
                Task.Run(async () =>
                {
                    try { await Task.WhenAll(_reminders.ToArray()); }
                    catch { }
                    finally { _reminders.Clear(); }
                });
            }
        }
    }
}
