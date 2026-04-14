using System.Windows;
using laba_10.Models;

namespace laba_10.Windows
{
    public partial class ProfileWindow : Window
    {
        private readonly Photographer _user;

        public ProfileWindow(Photographer user)
        {
            _user = user;
            InitializeComponent();

            LblName.Text = $"{_user.LastName} {_user.FirstName}";
            LblEmail.Text = _user.Email;
            LblPhone.Text = _user.PhoneNumber;
            LblDirection.Text = _user.PhotographyDirection;
            LblBirth.Text = _user.BirthDate.ToString("dd.MM.yyyy");
            LblPub.Text = _user.FirstPublicationDate.ToString("dd.MM.yyyy");
            LblRole.Text = _user.Role == "Admin" ? "Администратор" : "Пользователь";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}