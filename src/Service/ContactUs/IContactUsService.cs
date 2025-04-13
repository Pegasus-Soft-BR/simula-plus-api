using Domain.Common;
using Domain.DTOs;

namespace MockExams.Service
{
    public interface IContactUsService
    {
        Result<ContactUs> SendContactUs(ContactUs contactUs);
    }
}
