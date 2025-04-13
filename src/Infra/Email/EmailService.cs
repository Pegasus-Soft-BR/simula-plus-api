using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Domain;
using MockExams.Infra.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockExams.Infra.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly DbSet<User> _userRepository;
        protected readonly ApplicationDbContext _ctx;
        public IEmailTemplate Template { get; set; }

        public EmailService(IOptions<EmailSettings> emailSettings, ApplicationDbContext context, IEmailTemplate template)
        {
            _settings = emailSettings.Value;
            _ctx = context;
            _userRepository = _ctx.Users;
            Template = template;
        }

        public async Task SendToAdmins(string messageText, string subject)
        {
            var firstAdm = _ctx.Users.Where(u => u.Profile == Domain.Enums.Profile.Admin).FirstOrDefault();
            await Send(firstAdm.Email, firstAdm.Name, messageText, subject, copyAdmins: true);
        }

        public async Task Send(string emailRecipient, string nameRecipient, string messageText, string subject)
            => await Send(emailRecipient, nameRecipient, messageText, subject, false);

        public async Task Send(string emailRecipient, string nameRecipient, string messageText, string subject, bool copyAdmins = false)
        {
            var message = FormatEmail(emailRecipient, nameRecipient, messageText, subject, copyAdmins);
            try
            {
                using (var client = new SmtpClient())
                {
                    if (_settings.UseSSL)
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    }
                    
                    client.Connect(_settings.HostName, _settings.Port, _settings.UseSSL);
                    client.Authenticate(_settings.Username, _settings.Password);
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (System.Exception e)
            {
                //TODO: v2 implementar log para exceptions
                throw e;

            }
        }

        private MimeMessage FormatEmail(string emailRecipient, string nameRecipient, string messageText, string subject, bool copyAdmins)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.Username));
            message.To.Add(new MailboxAddress(nameRecipient, emailRecipient));

            if (copyAdmins)
            {
                var adminsEmails = GetAdminEmails();
                message.Cc.AddRange(adminsEmails);
            }

            message.Subject = subject;
            message.Body = new TextPart("HTML")
            {
                Text = messageText
            };
            return message;
        }

        private InternetAddressList GetAdminEmails()
        {
            var admins = _userRepository
                .Select(u => new User {
                    Name = u.Name,
                    Email = u.Email,
                    Profile = u.Profile
                })
                .Where(u => u.Profile == Domain.Enums.Profile.Admin)
                .ToList();

            InternetAddressList list = new InternetAddressList();
            foreach (var admin in admins)
            {
                list.Add(new MailboxAddress(admin.Name, admin.Email));
            }

            return list;
        }

        public async Task Test(string email, string name)
        {
            var subject = "MockExams - teste de email";
            var message = $"<p>Olá {name},</p> <p>Esse é um email de teste para verificar se o MockExams consegue fazer contato com você. Por favor avise o facilitador quando esse email chegar. Obrigado.</p>";
            await this.Send(email, name, message, subject);
        }

    }
}
