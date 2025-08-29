// TaskFlowAI.Application/Abstractions/IExportService.cs
using System.Collections.Generic;
using TaskFlowAI.Domain;

namespace TaskFlowAI.Application.Interfaces;

public interface IExportService
{
    string ExportToCsv(IEnumerable<TaskItem> items, string filePath);
    string ExportToPdf(IEnumerable<TaskItem> items, string filePath);
}