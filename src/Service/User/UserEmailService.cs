using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Domain;
using Domain.DTOs;
using MockExams.Infra.Email;

namespace MockExams.Service;

public class UserEmailService : IUserEmailService
{
    private const string ForgotPasswordTemplate = "ForgotPasswordTemplate";
    private const string ForgotPasswordTitle = "Esqueceu sua senha - MockExams";

    private readonly IEmailService _emailService;
    private readonly IEmailTemplate _emailTemplate;
    private readonly ServerSettings _serverSettings;

    public UserEmailService(IEmailService emailService, IEmailTemplate emailTemplate, IOptions<ServerSettings> serverSettings)
    {
        _emailService = emailService;
        _emailTemplate = emailTemplate;
        _serverSettings = serverSettings.Value;
    }

    public async Task SendEmailForgotMyPasswordToUserAsync(User user)
    {
        var vm = new
        {
            LinkForgotMyPassword = $"{_serverSettings.DefaultUrl}/forgotMyPassword2/{user.HashCodePassword}",
            User = user,
        };
        var html = await _emailTemplate.GenerateHtmlFromTemplateAsync(ForgotPasswordTemplate, vm);

        await _emailService.Send(user.Email, user.Name, html, ForgotPasswordTitle);
    }

    public void SendEmailAnonymizeNotifyAdms(UserAnonymizeDto dto)
    {
        var html = _emailTemplate.GenerateHtmlFromTemplateAsync("AnonymizeNotifyAdms", dto).Result;
        var title = "Anonimização de conta";
        _emailService.SendToAdmins(html, title);
    }

    public async Task SendEmailVerificationCode(User user)
    {
        var data = new { User = user };
        var html = await _emailTemplate.GenerateHtmlFromTemplateAsync("SendEmailVerificationCode", data);

        await _emailService.Send(user.Email, user.Name, html, "Verificação de email");
    }
}
