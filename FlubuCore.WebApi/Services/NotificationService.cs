using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FlubuCore.WebApi.Services
{
    public class NotificationService : INotificationService
    {
        private NotificationSettings _settings;

        public NotificationService(IOptions<NotificationSettings> notificationSettings)
        {
            _settings = notificationSettings.Value;
        }

        public async Task SendEmailAsync(string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.EmailFrom));
            foreach (var emailTo in _settings.EmailTo)
            {
                message.To.Add(new MailboxAddress(emailTo));
            }

            message.Subject = subject;
            message.Body = new TextPart(body);

            using (var client = new SmtpClient())
            {
                client.Connect(_settings.SmtpServerHost, _settings.SmtpServerPort, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
