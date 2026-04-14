using System.Windows;
using laba_10.Data;
using laba_10.Helpers;
using laba_10.Windows;

namespace laba_10
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var ctx = AppDbContext.GetContext();

            foreach (var user in ctx.Photographers)
            {
                if (user.Email == EmailBox.Text &&
                    PasswordHelper.VerifyPassword(PassBox.Password, user.PasswordHash))
                {
                    MessageBox.Show($"Добро пожаловать, {user.FirstName}!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    if (user.Role == "Admin")
                    {
                        new AdminWindow(user).Show();
                    }
                    else
                    {
                        new ProfileWindow(user).Show();
                    }
                    Hide();
                    return;
                }
            }

            MessageBox.Show("Неверный email или пароль!", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Recovery_Click(object sender, RoutedEventArgs e)
        {
            new PasswordRecoveryWindow().ShowDialog();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().ShowDialog();
        }
    }
}