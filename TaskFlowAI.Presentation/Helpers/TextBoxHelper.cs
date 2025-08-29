using System.Windows;
using System.Windows.Controls;

namespace TaskFlowAI.Presentation.Helpers
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(TextBoxHelper),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetPlaceholder(TextBox textBox) =>
            (string)textBox.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(TextBox textBox, string value) =>
            textBox.SetValue(PlaceholderProperty, value);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox) return;

            textBox.Loaded += (s, ev) => ShowPlaceholder(textBox);
            textBox.GotFocus += (s, ev) =>
            {
                if (textBox.Text == GetPlaceholder(textBox))
                {
                    textBox.Text = "";
                    textBox.Foreground = System.Windows.Media.Brushes.Black;
                }
            };
            textBox.LostFocus += (s, ev) => ShowPlaceholder(textBox);
        }

        private static void ShowPlaceholder(TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = GetPlaceholder(textBox);
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}