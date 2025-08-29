using System.Windows; // обязательно, для WPF Application
using Microsoft.Extensions.DependencyInjection;
using TaskFlowAI.Application.Interfaces;
using TaskFlowAI.Infrastructure.AI;
using TaskFlowAI.Infrastructure.Export;
using TaskFlowAI.Infrastructure.Persistence;
using TaskFlowAI.Infrastructure.Reminders;
using TaskFlowAI.Infrastructure.Settings;

namespace TaskFlowAI.Presentation
{
    public partial class App : System.Windows.Application
    {
        public static ServiceProvider Services = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаём DI контейнер
            var sc = new ServiceCollection();

            // DB + репозиторий
            sc.AddDbContext<AppDbContext>(ServiceLifetime.Singleton);
            sc.AddSingleton<ITaskRepository, TaskRepository>();

            // Сервисы
            sc.AddSingleton<ISettingsService, SettingsService>();
            sc.AddSingleton<IAiAnalyzer, HeuristicAiAnalyzer>();
            sc.AddSingleton<IExportService, ExportService>();
            sc.AddSingleton<IReminderScheduler, ReminderScheduler>();

            // VM + окно
            sc.AddSingleton<MainViewModel>();
            sc.AddSingleton<MainWindow>();

            Services = sc.BuildServiceProvider();

            // Создаём базу
            var db = Services.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            // Показываем главное окно
            var main = Services.GetRequiredService<MainWindow>();
            MainWindow = main;
            main.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Services.Dispose();
            base.OnExit(e);
        }
    }
}
