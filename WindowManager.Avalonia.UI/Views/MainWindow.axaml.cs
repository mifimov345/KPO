using Avalonia.Controls;
using WindowManager.Avalonia.UI.ViewModels;

namespace WindowManager.Avalonia.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public class MainWindowViewModel : ViewModelBase
        {
            public string Text => "Welcome to Avalonia";
        }
    }
}