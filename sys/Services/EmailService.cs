using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace sys.Services {
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            System.Diagnostics.Debug.WriteLine($"[EmailService] Enter SendAsync, to={message.Destination}");
            var mail = new MailMessage("no-reply@yourdomain.com", message.Destination)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.SendMailAsync(mail);
                    System.Diagnostics.Debug.WriteLine("[EmailService] Mail sent successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[EmailService] ERROR: {ex}");
                    throw;
                }
            }
        }
    }
}
