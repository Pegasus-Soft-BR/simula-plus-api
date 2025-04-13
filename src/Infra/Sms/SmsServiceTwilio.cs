using Microsoft.Extensions.Options;
using MockExams.Helper;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace MockExams.Infra.Sms;

public class SmsServiceTwilio: ISmsService
{
    private SmsSettingsTwillio _settings { get; set; }

    public SmsServiceTwilio(IOptions<SmsSettingsTwillio> settings)
    {
        _settings = settings.Value;

        TwilioClient.Init(_settings.AccountSID, _settings.AuthToken);
    }

    public async Task SendMessage(string phoneNumber, string message)
    {
        if (!_settings.IsActive) throw new SmsDisabledException("O serviço SMS está desativado no appsettings.");

        phoneNumber = PhoneHelper.FormatPhoneNumber(phoneNumber);

        var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber));

        messageOptions.MessagingServiceSid = _settings.MessagingServiceSid;
        messageOptions.Body = message;

        var result = await MessageResource.CreateAsync(messageOptions);

        Console.Write(result.ToString());
    }
}






