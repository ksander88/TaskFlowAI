using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;                        // для Path
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;                   // для Application.Current
using System.Windows.Data;
using TaskFlowAI.Application.Interfaces;
using TaskFlowAI.Domain;
using DomainTaskStatus = TaskFlowAI.Domain.TaskStatus;  // алиас для твоего TaskStatus

using TaskFlowAI.Presentation;
using TaskFlowAI.Application.Models;

public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITaskRepository _repo;
        private readonly IAiAnalyzer _ai;
        private readonly ISettingsService _settings;
        private readonly IExportService _export;

        public ObservableCollection<TaskItem> Tasks { get; } = new();
        public ICollectionView TasksView { get; }

        // Фильтры
        private string _search = string.Empty;
        public string Search
        {
            get => _search;
            set
            {
                _search = value;
                OnPropertyChanged();
                TasksView.Refresh();
            }
        }

        public Priority? FilterPriority { get; set; }
        public DomainTaskStatus? FilterStatus { get; set; }
        public string? FilterCategory { get; set; }

        // Выбранная задача
        private TaskItem? _selected;
        public TaskItem? Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        // Команды
        public RelayCommand AddCmd { get; }
        public RelayCommand EditCmd { get; }
        public RelayCommand DeleteCmd { get; }
        public RelayCommand AnalyzeCmd { get; }
        public RelayCommand ClearFiltersCmd { get; }
        public RelayCommand ExportCsvCmd { get; }
        public RelayCommand ExportPdfCmd { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel(ITaskRepository repo, IAiAnalyzer ai, ISettingsService settings, IExportService export)
        {
            _repo = repo;
            _ai = ai;
            _settings = settings;
            _export = export;

            TasksView = CollectionViewSource.GetDefaultView(Tasks);
            TasksView.Filter = FilterPredicate;

            AddCmd = new RelayCommand(async _ => await AddAsync());
            EditCmd = new RelayCommand(async _ => await EditAsync(), _ => Selected != null);
            DeleteCmd = new RelayCommand(async _ => await DeleteAsync(), _ => Selected != null);
            AnalyzeCmd = new RelayCommand(_ => Analyze(), _ => Selected != null);
            ClearFiltersCmd = new RelayCommand(_ =>
            {
                Search = "";
                FilterPriority = null;
                FilterStatus = null;
                FilterCategory = null;
                TasksView.Refresh();
            });
            ExportCsvCmd = new RelayCommand(_ => ExportCsv());
            ExportPdfCmd = new RelayCommand(_ => ExportPdf());

            _ = LoadAsync();
        }

        private bool FilterPredicate(object obj)
        {
            if (obj is not TaskItem t) return false;
            bool matches = true;

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var s = Search.Trim().ToLowerInvariant();
                matches &= (t.Title?.ToLowerInvariant().Contains(s) == true)
                        || (t.Description?.ToLowerInvariant().Contains(s) == true);
            }

            if (FilterPriority.HasValue) matches &= t.Priority == FilterPriority.Value;
            if (FilterStatus.HasValue) matches &= t.Status == FilterStatus.Value;
            if (!string.IsNullOrWhiteSpace(FilterCategory))
                matches &= (t.Category?.Equals(FilterCategory, StringComparison.OrdinalIgnoreCase) == true);

            return matches;
        }
        private async Task LoadAsync()
        {
            try
            {
                Tasks.Clear();
                var all = await _repo.GetAllAsync();
                foreach (var t in all) Tasks.Add(t);
                TasksView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddAsync()
        {
            var vm = new TaskEditViewModel();
            var dlg = new TaskDialog { DataContext = vm, Owner = Application.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var entity = vm.ToEntity();
                    if (string.IsNullOrWhiteSpace(entity.Title))
                    {
                        MessageBox.Show("Название обязательно.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    await _repo.AddAsync(entity);
                    Tasks.Insert(0, entity);
                    TasksView.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task EditAsync()
        {
            if (Selected == null) return;
            var vm = TaskEditViewModel.FromEntity(Selected);
            var dlg = new TaskDialog { DataContext = vm, Owner = Application.Current.MainWindow };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var updated = vm.ToEntity();
                    updated.Id = Selected.Id;
                    await _repo.UpdateAsync(updated);

                    var idx = Tasks.IndexOf(Selected);
                    Tasks[idx] = updated;
                    Selected = updated;
                    TasksView.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка редактирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteAsync()
        {
            if (Selected == null) return;
            if (MessageBox.Show($"Удалить “{Selected.Title}”?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    await _repo.DeleteAsync(Selected.Id);
                    Tasks.Remove(Selected);
                    Selected = null;
                    TasksView.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Analyze()
        {
            if (Selected == null) return;
            var (p, h, expl) = _ai.Analyze(Selected);
            Selected.Priority = p;
            if (h.HasValue) Selected.EstimatedHours = h.Value;
            MessageBox.Show(expl, "Анализ ИИ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportCsv()
        {
            try
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "tasks_export.csv");
                _export.ExportToCsv(TasksView.Cast<TaskItem>(), path);
                MessageBox.Show($"CSV экспортирован: {path}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта CSV: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExportPdf()
        {
            try
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "tasks_export.pdf");
                _export.ExportToPdf(TasksView.Cast<TaskItem>(), path);
                MessageBox.Show($"PDF экспортирован: {path}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта PDF: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Сохранение/загрузка позиции окна
        public async Task<WindowSettings> LoadWindowAsync() => await _settings.LoadAsync();
        public async Task SaveWindowAsync(WindowSettings s) => await _settings.SaveAsync(s);

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
