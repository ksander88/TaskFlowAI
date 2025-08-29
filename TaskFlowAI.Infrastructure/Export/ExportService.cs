// TaskFlowAI.Infrastructure/Export/ExportService.cs
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TaskFlowAI.Application.Interfaces;
using TaskFlowAI.Domain;
using System.Collections.Generic;

namespace TaskFlowAI.Infrastructure.Export;

public sealed class ExportService : IExportService
{
    public string ExportToCsv(IEnumerable<TaskItem> items, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id;Title;Description;Priority;Status;Category;DueDate;EstimatedHours;CreatedAt;UpdatedAt");
        foreach (var t in items)
        {
            var line = string.Join(';', new[]
            {
                t.Id.ToString(),
                Escape(t.Title),
                Escape(t.Description),
                t.Priority.ToString(),
                t.Status.ToString(),
                t.Category,
                t.DueDate?.ToString("yyyy-MM-dd") ?? "",
                t.EstimatedHours?.ToString(CultureInfo.InvariantCulture) ?? "",
                t.CreatedAt.ToString("u"),
                t.UpdatedAt.ToString("u")
            });
            sb.AppendLine(line);
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        return filePath;
    }

    private static string Escape(string s) => (s ?? string.Empty).Replace(";", ",").Replace("\r", " ").Replace("\n", " ");

    public string ExportToPdf(IEnumerable<TaskItem> items, string filePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var list = items?.ToList() ?? new List<TaskItem>();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Header().Text("TaskFlowAI — Export").SemiBold().FontSize(20);
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2); // Title
                        cols.RelativeColumn(3); // Description
                        cols.RelativeColumn();  // Priority
                        cols.RelativeColumn();  // Status
                        cols.RelativeColumn();  // Category
                        cols.RelativeColumn();  // DueDate
                    });

                    // header
                    table.Header(header =>
                    {
                        header.Cell().Element(CellHeader).Text("Title");
                        header.Cell().Element(CellHeader).Text("Description");
                        header.Cell().Element(CellHeader).Text("Priority");
                        header.Cell().Element(CellHeader).Text("Status");
                        header.Cell().Element(CellHeader).Text("Category");
                        header.Cell().Element(CellHeader).Text("DueDate");

                        static IContainer CellHeader(IContainer c) =>
                            c.PaddingVertical(5).Background(Colors.Grey.Lighten3).BorderBottom(1);
                    });

                    foreach (var t in list)
                    {
                        table.Cell().Text(t.Title);
                        table.Cell().Text(t.Description);
                        table.Cell().Text(t.Priority.ToString());
                        table.Cell().Text(t.Status.ToString());
                        table.Cell().Text(t.Category);
                        table.Cell().Text(t.DueDate?.ToString("yyyy-MM-dd") ?? "");
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated ").Italic();
                    x.Span(DateTime.Now.ToString("u"));
                });
            });
        }).GeneratePdf(filePath);

        return filePath;
    }
}
