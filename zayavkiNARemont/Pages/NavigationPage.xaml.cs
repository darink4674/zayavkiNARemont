using System.Windows;
using System.Windows.Controls;
using zayavkiNARemont.Conn;

namespace zayavkiNARemont.Pages
{
    public partial class NavigationPage : Page
    {
        private users _currentUser;

        public NavigationPage(users user)
        {
            InitializeComponent();
            _currentUser = user;

            tbUser.Text = user.full_name;
            tbRole.Text = $"Роль: {user.roles.name}";

            LoadMenu();
        }

        private void LoadMenu()
        {
            string role = _currentUser.roles.name;

            // Кнопки для всех
            AddButton("СПИСОК ЗАЯВОК", () => NavigationService.Navigate(new RequestsPage(_currentUser)));

            // Клиент может создавать заявки
            if (role == "client" || role == "manager" || role == "admin")
            {
                AddButton("НОВАЯ ЗАЯВКА", () => NavigationService.Navigate(new AddRequestPage(_currentUser)));
            }
        }

        private void AddButton(string text, System.Action action)
        {
            Button btn = new Button();
            btn.Content = text;
            btn.Width = 200;
            btn.Height = 60;
            btn.Margin = new Thickness(10);
            btn.FontSize = 14;
            btn.FontWeight = FontWeights.Bold;
            btn.Background = System.Windows.Media.Brushes.White;
            btn.BorderBrush = System.Windows.Media.Brushes.LightGray;
            btn.Click += (s, e) => action();
            pnlMenu.Children.Add(btn);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }
    }
}