using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using laba_10.Data;
using laba_10.Helpers;
using laba_10.Models;

namespace laba_10.Windows
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email.Trim(),
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string cleanPhone = Regex.Replace(phone, @"[^\d]", "");
            return cleanPhone.Length >= 10 && cleanPhone.Length <= 12;
        }

        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            name = name.Trim();

            if (name.Length < 2 || name.Length > 50)
                return false;

            return Regex.IsMatch(name, @"^[а-яА-ЯёЁa-zA-Z\s\-]+$");
        }

        private bool IsValidBirthDate(DateTime? date, out string errorMessage)
        {
            errorMessage = "";

            if (!date.HasValue)
            {
                errorMessage = "Выберите дату рождения";
                return false;
            }

            var birthDate = date.Value.Date;
            var today = DateTime.Today;
            var minDate = today.AddYears(-120);
            var maxDate = today.AddYears(-14);

            if (birthDate > today)
            {
                errorMessage = "Дата рождения не может быть в будущем";
                return false;
            }

            if (birthDate < minDate)
            {
                errorMessage = "Недопустимая дата рождения (слишком давно)";
                return false;
            }

            if (birthDate > maxDate)
            {
                errorMessage = "Вам должно быть не менее 14 лет";
                return false;
            }

            return true;
        }

        private bool IsValidPassword(string password, out string errorMessage)
        {
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Введите пароль";
                return false;
            }

            if (password.Length < 6)
            {
                errorMessage = "Пароль должен содержать не менее 6 символов";
                return false;
            }

            if (password.Length > 50)
            {
                errorMessage = "Пароль не должен превышать 50 символов";
                return false;
            }

            if (!Regex.IsMatch(password, @"[a-zA-Zа-яА-ЯёЁ]") ||
                !Regex.IsMatch(password, @"\d"))
            {
                errorMessage = "Пароль должен содержать буквы и цифры";
                return false;
            }

            return true;
        }

        private bool IsValidPublicationDate(DateTime? date, out string errorMessage)
        {
            errorMessage = "";

            if (!date.HasValue)
            {
                errorMessage = "Выберите дату первой публикации";
                return false;
            }

            var pubDate = date.Value.Date;
            var today = DateTime.Today;

            if (pubDate > today)
            {
                errorMessage = "Дата не может быть в будущем";
                return false;
            }

            if (pubDate.Year < 1900)
            {
                errorMessage = "Недопустимая дата";
                return false;
            }

            return true;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            ClearErrors();

            bool hasErrors = false;
            var ctx = AppDbContext.GetContext(); 
            if (!IsValidName(TxtFirstName.Text))
            {
                ShowError(TxtFirstName, "Введите корректное имя (2-50 букв)");
                hasErrors = true;
            }
            if (!IsValidName(TxtLastName.Text))
            {
                ShowError(TxtLastName, "Введите корректную фамилию (2-50 букв)");
                hasErrors = true;
            }
            if (!IsValidBirthDate(DPBirth.SelectedDate, out string birthError))
            {
                ShowError(DPBirth, birthError);
                hasErrors = true;
            }
            if (!IsValidEmail(TxtEmail.Text))
            {
                ShowError(TxtEmail, "Введите корректный email (пример: name@domain.com)");
                hasErrors = true;
            }
            else
            {
                if (ctx.Photographers.Any(u => u.Email == TxtEmail.Text.Trim()))
                {
                    ShowError(TxtEmail, "Этот email уже зарегистрирован");
                    hasErrors = true;
                }
            }

            if (!IsValidPhone(TxtPhone.Text))
            {
                ShowError(TxtPhone, "Введите корректный номер (10-12 цифр)");
                hasErrors = true;
            }
            if (string.IsNullOrWhiteSpace(TxtDirection.Text) || TxtDirection.Text.Trim().Length < 3)
            {
                ShowError(TxtDirection, "Укажите направление (минимум 3 символа)");
                hasErrors = true;
            }
            if (!IsValidPublicationDate(DPPub.SelectedDate, out string pubError))
            {
                ShowError(DPPub, pubError);
                hasErrors = true;
            }
            if (!IsValidPassword(PBPass.Password, out string passError))
            {
                ShowError(PBPass, passError);
                hasErrors = true;
            }

            if (hasErrors)
            {
                MessageBox.Show("Исправьте ошибки в форме", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var user = new Photographer
            {
                FirstName = TxtFirstName.Text.Trim(),
                LastName = TxtLastName.Text.Trim(),
                BirthDate = DPBirth.SelectedDate.Value,
                Email = TxtEmail.Text.Trim().ToLower(),
                PasswordHash = PasswordHelper.HashPassword(PBPass.Password),
                PhoneNumber = TxtPhone.Text.Trim(),
                PhotographyDirection = TxtDirection.Text.Trim(),
                FirstPublicationDate = DPPub.SelectedDate.Value,
                Role = IsAdmin.IsChecked == true ? "Admin" : "User"
            };

            ctx.Photographers.Add(user);
            ctx.SaveChanges();

            MessageBox.Show("Регистрация прошла успешно!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void ShowError(TextBox control, string message)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.ToolTip = message;
        }

        private void ShowError(PasswordBox control, string message)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.ToolTip = message;
        }

        private void ShowError(DatePicker control, string message)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.ToolTip = message;
        }

        private void ClearErrors()
        {
            TxtFirstName.BorderBrush = System.Windows.Media.Brushes.Gray;
            TxtLastName.BorderBrush = System.Windows.Media.Brushes.Gray;
            TxtEmail.BorderBrush = System.Windows.Media.Brushes.Gray;
            TxtPhone.BorderBrush = System.Windows.Media.Brushes.Gray;
            TxtDirection.BorderBrush = System.Windows.Media.Brushes.Gray;
            PBPass.BorderBrush = System.Windows.Media.Brushes.Gray;
            DPBirth.BorderBrush = System.Windows.Media.Brushes.Gray;
            DPPub.BorderBrush = System.Windows.Media.Brushes.Gray;

            TxtFirstName.ToolTip = null;
            TxtLastName.ToolTip = null;
            TxtEmail.ToolTip = null;
            TxtPhone.ToolTip = null;
            TxtDirection.ToolTip = null;
            PBPass.ToolTip = null;
            DPBirth.ToolTip = null;
            DPPub.ToolTip = null;
        }
    }
}