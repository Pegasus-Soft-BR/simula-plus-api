using Domain.DTOs;
using System.Threading.Tasks;

namespace MockExams.Service
{
    public interface IContactUsEmailService
    {
        Task SendEmailContactUs(ContactUs contactUs);
    }
}
