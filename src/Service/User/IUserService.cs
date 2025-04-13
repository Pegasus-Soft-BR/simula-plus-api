using Domain;
using Domain.Common;
using Domain.DTOs;
using Google.Apis.Auth;
using MockExams.Service.Generic;
using System;
using System.Threading.Tasks;

namespace MockExams.Service
{
    public interface IUserService : IBaseService<User>
    {
        Result<User> AuthenticationByEmailAndPassword(User user);
        Task<Result<User>> AuthenticationByGoogleIdToken(string idToken);

        bool IsValidPassword(User user, string decryptedPass);
        new Result<User> Update(User user);
        Result<User> ValidOldPasswordAndChangeUserPassword(User user, string newPassword);
        Result<User> ChangeUserPassword(User user, string newPassword);
        Result GenerateHashCodePasswordAndSendEmailToUser(string email);
        Result ConfirmHashCodePassword(string hashCodePassword);


        User FindById(Guid IdUser, Guid IdAdmin);
        Task<UserEmailAndPhoneVerifiedDto> IsEmailAndPhoneVerified(string email, string phone);
        Task SendEmailVerificationCode(string email);
        Task SendPhoneVerificationCode(string phone);
        Task<User> EmailVerification(string email, string verificationCode);
        Task<User> PhoneVerification(string phone, string verificationCode);
        
    }
}
