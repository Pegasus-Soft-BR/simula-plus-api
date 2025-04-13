using System.Threading.Tasks;

namespace MockExams.Infra.Sms;

public interface ISmsService
{
    Task SendMessage(string phoneNumber, string message);
}