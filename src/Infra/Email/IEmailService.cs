using System.Threading.Tasks;

namespace MockExams.Infra.Email
{
    public interface IEmailService
    {
        Task SendToAdmins(string messageText, string subject);
        Task Send(string emailRecipient, string nameRecipient, string messageText, string subject);
        Task Send(string emailRecipient, string nameRecipient, string messageText, string subject, bool copyAdmins = false);
        Task Test(string email, string name);
        public IEmailTemplate Template { get; set; }
    }
}
