using System.Threading.Tasks;
using Domain;
using Domain.DTOs;

namespace MockExams.Service;

public interface IUserEmailService
{
    Task SendEmailForgotMyPasswordToUserAsync(User user);
    void SendEmailAnonymizeNotifyAdms(UserAnonymizeDto dto);
    Task SendEmailVerificationCode(User user);
}
