using LaTiQ.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LaTiQ.Core.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendMailAsync(string email, string subject, string message)
        {
            string? sender = _configuration["EmailConfiguration:Sender"];
            string? password = _configuration["EmailConfiguration:Password"];
            string? smtpServer = _configuration["EmailConfiguration:SmtpServer"];
            int port = int.Parse(_configuration["EmailConfiguration:Port"]);

            // Console.WriteLine("sender = " + sender);
            // Console.WriteLine("password = " + password);

            var client = new SmtpClient(smtpServer, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(sender, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(sender),
                Subject = subject,
                Body = message, // Đây là nơi bạn truyền đoạn mã HTML vào
                IsBodyHtml = true // Thiết lập IsBodyHtml để cho phép nội dung HTML
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}
