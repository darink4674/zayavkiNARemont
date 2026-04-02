using System.Windows;
using System.Windows.Controls;
using zayavkiNARemont.Pages;

namespace zayavkiNARemont
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }
    }
}