using System.Windows;

namespace TaskFlowAI.Presentation
{
    public partial class TaskDialog : Window
    {
        public TaskDialog() => InitializeComponent();

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TaskEditViewModel vm)
            {
                if (string.IsNullOrWhiteSpace(vm.Title))
                {
                    MessageBox.Show("Название обязательно.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DialogResult = true;
            Close();
        }
    }
}
