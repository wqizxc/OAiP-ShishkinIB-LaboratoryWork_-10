using System.Windows;
using laba_10.Data;
using laba_10.Helpers;

namespace laba_10.Windows
{
    public partial class PasswordRecoveryWindow : Window
    {
        private string? _generatedCode;

        public PasswordRecoveryWindow()
        {
            InitializeComponent();
        }

        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            var ctx = AppDbContext.GetContext();
            var user = ctx.Photographers.FirstOrDefault(u => u.Email == EmailBox.Text);

            if (user == null)
            {
                MessageBox.Show("Пользователь с таким Email не найден!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _generatedCode = EmailService.SendResetCode(user.Email);
                MessageBox.Show("Код отправлен на ваш Email!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (_generatedCode == null)
            {
                MessageBox.Show("Сначала отправьте код!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CodeBox.Text != _generatedCode)
            {
                MessageBox.Show("Неверный код!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassBox.Password))
            {
                MessageBox.Show("Введите новый пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ctx = AppDbContext.GetContext();
            var user = ctx.Photographers.First(u => u.Email == EmailBox.Text);
            user.PasswordHash = PasswordHelper.HashPassword(NewPassBox.Password);
            ctx.SaveChanges();

            MessageBox.Show("Пароль успешно изменён!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}