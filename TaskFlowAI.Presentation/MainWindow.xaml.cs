using System;
using System.Linq;
using System.Windows;
using TaskFlowAI.Application.Interfaces;
using TaskFlowAI.Application.Models; // для WindowSettings

namespace TaskFlowAI.Presentation;

public partial class MainWindow : Window
{
    private MainViewModel VM => (MainViewModel)DataContext;

    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var s = await VM.LoadWindowAsync();
            Left = s.Left;
            Top = s.Top;
            Width = s.Width;
            Height = s.Height;
            if (s.Maximized) WindowState = WindowState.Maximized;
        }
        catch
        {
            // игнорируем ошибки загрузки настроек окна
        }
    }

    private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            var s = new WindowSettings
            {
                Left = this.RestoreBounds.Left,
                Top = this.RestoreBounds.Top,
                Width = this.RestoreBounds.Width,
                Height = this.RestoreBounds.Height,
                Maximized = this.WindowState == WindowState.Maximized
            };
            await VM.SaveWindowAsync(s);
        }
        catch
        {
            // игнорируем ошибки сохранения настроек окна
        }
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (VM.EditCmd.CanExecute(null))
            VM.EditCmd.Execute(null);
    }
}
