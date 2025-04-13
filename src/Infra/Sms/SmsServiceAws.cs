using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;
using MockExams.Helper;
using System.Threading.Tasks;

namespace MockExams.Infra.Sms;

public class SmsServiceAws : ISmsService
{
    private SmsSettings _settings { get; set; }
    private AmazonSimpleNotificationServiceClient _sns;

    public SmsServiceAws(IOptions<SmsSettings> settings)
    {
        _settings = settings.Value;
        _sns = new AmazonSimpleNotificationServiceClient(_settings.AccessKey, _settings.SecretKey, Amazon.RegionEndpoint.SAEast1);
    }

    public async Task SendMessage(string phoneNumber, string message)
    {
        if (!_settings.IsActive) throw new SmsDisabledException("O serviço SMS está desativado no appsettings.");

        phoneNumber = PhoneHelper.FormatPhoneNumber(phoneNumber);

        var request = new PublishRequest
        {
            PhoneNumber = phoneNumber,
            Message = message
        };

        await _sns.PublishAsync(request);
    }
}






