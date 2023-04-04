namespace AstView;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    private void OnDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is TextBox textBox)
        {
            var pos = textBox.GetCharacterIndexFromPoint(e.GetPosition(textBox), snapToText: true);
            if (((ViewModel)this.DataContext).TrySelect(pos, out var start, out var length))
            {
                textBox.Select(start, length);
            }
        }
    }
}
