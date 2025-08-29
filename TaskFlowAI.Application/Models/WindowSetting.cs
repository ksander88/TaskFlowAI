// TaskFlowAI.Application/Models/WindowSettings.cs
namespace TaskFlowAI.Application.Models;

public class WindowSettings
{
    public double Width { get; set; } = 1000;
    public double Height { get; set; } = 700;
    public double Left { get; set; } = 100;
    public double Top { get; set; } = 100;
    public bool Maximized { get; set; }
}
