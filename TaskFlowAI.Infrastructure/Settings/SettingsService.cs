// TaskFlowAI.Infrastructure/Settings/SettingsService.cs
using System.Text.Json;
using TaskFlowAI.Application.Interfaces;
using TaskFlowAI.Application.Models;

namespace TaskFlowAI.Infrastructure.Settings;

public sealed class SettingsService : ISettingsService
{
    private readonly string _filePath;

    public SettingsService()
    {
        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dir = Path.Combine(local, "TaskFlowAI");
        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "settings.json");
    }

    public async Task<WindowSettings> LoadAsync()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var s = JsonSerializer.Deserialize<WindowSettings>(json);
                return s ?? new WindowSettings();
            }
        }
        catch { /* игнорируем, вернём дефолт */ }
        return new WindowSettings();
    }

    public async Task SaveAsync(WindowSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}