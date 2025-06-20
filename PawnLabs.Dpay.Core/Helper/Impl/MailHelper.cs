using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using PawnLabs.Dpay.Core.Option;

namespace PawnLabs.Dpay.Core.Helper.Impl
{
    public class MailHelper : IMailHelper
    {
        private MailConfiguration mailConfiguration;

        public MailHelper(IOptions<MailConfiguration> mailConfiguration)
        {
            this.mailConfiguration = mailConfiguration.Value;
        }

        public async Task<string> SendVerificationEmail(string toEmail)
        {
            var verificationCode = GenerateVerificationCode();

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(mailConfiguration.MailAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Dpay - Verification Mail";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $@"
                        <div style='font-family:Arial, sans-serif; font-size:16px; color:#333;'>
                            <p>Merhaba,</p>
                            <p>Giriş veya kayıt işlemini tamamlamak için aşağıdaki doğrulama kodunu kullanabilirsiniz:</p>
                            <div style='margin:20px 0; padding:15px; background-color:#f2f2f2; text-align:center; font-size:24px; font-weight:bold; color:#2c3e50; border-radius:8px;'>
                                {verificationCode}
                            </div>
                            <p>Bu kod 5 dakika boyunca geçerlidir.</p>
                            <p>Teşekkürler,<br/>SoLuck Ekibi</p>
                        </div>"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(mailConfiguration.MailAddress, mailConfiguration.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return verificationCode;
        }

        #region Inner Functions

        private string GenerateVerificationCode()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = 6;

            return new string(Enumerable.Repeat(chars, length).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        #endregion
    }
}
