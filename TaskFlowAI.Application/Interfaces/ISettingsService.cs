// TaskFlowAI.Application/Interfaces/ISettingsService.cs
using System.Threading.Tasks;
using TaskFlowAI.Application.Models;

namespace TaskFlowAI.Application.Interfaces;

public interface ISettingsService
{
    Task<WindowSettings> LoadAsync();
    Task SaveAsync(WindowSettings settings);
}
