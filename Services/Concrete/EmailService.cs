using FinexaApi.Services.Abstract;
using System.Net;
using System.Net.Mail;

namespace FinexaApi.Services.Concrete
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            try
            {
                var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
                var smtpPortStr = Environment.GetEnvironmentVariable("SMTP_PORT");
                var smtpEnableSslStr = Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL");
                var smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL");
                var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

                if (string.IsNullOrWhiteSpace(smtpHost) ||
                    string.IsNullOrWhiteSpace(smtpPortStr) ||
                    string.IsNullOrWhiteSpace(smtpEnableSslStr) ||
                    string.IsNullOrWhiteSpace(smtpEmail) ||
                    string.IsNullOrWhiteSpace(smtpPassword))
                {
                    return false;
                }

                int smtpPort = int.Parse(smtpPortStr);
                bool smtpEnableSsl = bool.Parse(smtpEnableSslStr);

                var mail = new MailMessage();
                mail.From = new MailAddress(smtpEmail, "Finexa Destek");
                mail.To.Add(toEmail);
                mail.Subject = "🔐 Şifre Sıfırlama Talebiniz";

                mail.IsBodyHtml = true;
                mail.Body = $@"
                <div style='font-family:Arial,sans-serif; font-size:16px; color:#333; text-align:left;'>
                    <h2>Merhaba,</h2>
                    <p>Hesabınız için bir <strong>şifre sıfırlama</strong> talebi aldık.</p>
                    <p>Aşağıdaki butona tıklayarak şifrenizi sıfırlayabilirsiniz:</p>

                    <div style='text-align:center; margin:30px 0;'>
                        <a href='{resetLink}' style='background-color:#4CAF50; color:white; padding:12px 24px; text-decoration:none; border-radius:5px; display:inline-block;'>
                            Şifremi Sıfırla
                        </a>
                    </div>

                    <p>Eğer bu talebi siz yapmadıysanız, lütfen bu e-postayı dikkate almayınız.</p>
                    <br>
                    <p>İyi günler dileriz,<br><strong>Finexa Destek Ekibi</strong></p>
                </div>";

                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                    smtpClient.EnableSsl = smtpEnableSsl;
                    await smtpClient.SendMailAsync(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
