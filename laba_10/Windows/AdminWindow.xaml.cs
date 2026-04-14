using System.Windows;
using System.Windows.Controls;
using laba_10.Data;
using laba_10.Models;

namespace laba_10.Windows
{
    public partial class AdminWindow : Window
    {
        private readonly Photographer _admin;

        public AdminWindow(Photographer admin)
        {
            _admin = admin;
            InitializeComponent();
            AdminName.Text = $"{admin.LastName} {admin.FirstName}";
            LoadUsers();
        }

        private void LoadUsers()
        {
            var ctx = AppDbContext.GetContext();
            UsersGrid.ItemsSource = ctx.Photographers.ToList();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegistrationWindow();
            window.ShowDialog();
            LoadUsers();
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is not Photographer user)
            {
                MessageBox.Show("Выберите пользователя для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (user.Id == _admin.Id)
            {
                MessageBox.Show("Нельзя удалить свою учетную запись!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить пользователя {user.FirstName} {user.LastName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var ctx = AppDbContext.GetContext();
                ctx.Photographers.Remove(user);
                ctx.SaveChanges();
                LoadUsers();
                MessageBox.Show("Пользователь удален", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}