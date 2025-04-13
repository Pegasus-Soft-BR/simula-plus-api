using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using MockExams.Helper;
using ShareBook.Domain.Exceptions;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vonage;
using Vonage.Request;

namespace MockExams.Infra.Sms;

public class SmsServiceVonade: ISmsService
{
    private SmsSettings _settings { get; set; }
    private VonageClient _vonageClient { get; set; }

    public SmsServiceVonade(IOptions<SmsSettings> settings)
    {
        _settings = settings.Value;

        var credentials = Credentials.FromApiKeyAndSecret(
            _settings.AccessKey,
            _settings.SecretKey
            );

        _vonageClient = new VonageClient(credentials);
    }

    public async Task SendMessage(string phoneNumber, string message)
    {
        if (!_settings.IsActive) throw new SmsDisabledException("O serviço SMS está desativado no appsettings.");

        phoneNumber = PhoneHelper.FormatPhoneNumber(phoneNumber);

        await _vonageClient.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest()
        {
            To = phoneNumber,
            From = "Salve Elas App",
            Text = message
        });
    }
}






