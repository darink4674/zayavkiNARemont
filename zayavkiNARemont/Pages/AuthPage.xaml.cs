using System.Linq;
using System.Windows;
using System.Windows.Controls;
using zayavkiNARemont.Conn;

namespace zayavkiNARemont.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string pass = pbPassword.Password.Trim();

            if (login.Length == 0 || pass.Length == 0)
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            var user = DB.equipmentRepairDBEntities.users
                .FirstOrDefault(x => x.login == login && x.password_hash == pass && x.is_active == true);

            if (user != null)
            {
                NavigationService.Navigate(new NavigationPage(user));
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }
    }
}