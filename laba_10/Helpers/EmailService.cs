using System.Net;
using System.Net.Mail;
using System.Text;

namespace laba_10.Helpers
{
    public static class EmailService
    {
        private const string SmtpHost = "smtp.mail.ru";
        private const int SmtpPort = 587;
        private const string SenderEmail = "ilyashishkin00@mail.ru";
        private const string SenderAppPassword = "34BMfkF5Gv66JZoksSMY";

        public static string SendResetCode(string toEmail)
        {
            var code = new Random().Next(100000, 999999).ToString();

            using var client = new SmtpClient(SmtpHost, SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(SenderEmail, SenderAppPassword)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(SenderEmail, "Восстановление доступа"),
                Subject = "Код подтверждения для смены пароля",
                Body = $"<h1>Ваш код: {code}</h1><p>Введите его в приложении для смены пароля.</p>",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };

            mail.To.Add(toEmail);
            client.Send(mail);
            return code;
        }
    }
}