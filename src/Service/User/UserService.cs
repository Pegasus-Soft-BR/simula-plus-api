using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Common;
using Domain.DTOs;
using Domain.Exceptions;
using MockExams.Helper.Crypto;
using MockExams.Infra.Database;
using MockExams.Infra.Database.UoW;
using MockExams.Infra.Sms;
using MockExams.Service.Generic;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace MockExams.Service;

public class UserService : BaseService<User>, IUserService
{
    private readonly IUserEmailService _userEmailService;
    private readonly IMapper _mapper;
    private ISmsService _smsService { get; set; }

    #region Public

    public UserService(ApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IValidator<User> validator, IMapper mapper,
        IUserEmailService userEmailService, ISmsService smsService) : base(context, unitOfWork, validator)
    {
        _userEmailService = userEmailService;
        _mapper = mapper;
        _smsService = smsService;
    }

    public Result<User> AuthenticationByEmailAndPassword(User user)
    {
        var result = Validate(user, x => x.Email, x => x.Password);

        string decryptedPass = user.Password;

        user = _entity.FirstOrDefault(e => e.Email == user.Email);

        if (user == null)
        {
            result.Messages.Add("Não encontramos esse email no MockExams. Você já se cadastrou?");
            return result;
        }

        if (user.IsBruteForceLogin())
        {
            result.Messages.Add("Login bloqueado por 30 segundos, para proteger sua conta.");
            return result;
        }

        // if (!user.IsEmailVerified)
        //     result.Messages.Add("Email ainda não foi verificado. Você precisa verificar sua caixa de entrada de email e confirmar seu email.");

        if (!user.IsPhoneVerified)
            result.Messages.Add($"Telefone ainda não foi verificado. Você precisa verificar seu SMS e confirmar seu telefone. >> {user.Phone}");

        if(!result.Success) return result;

        // persiste última tentativa de login ANTES do SUCESSO ou FALHA pra ter métrica de
        // verificação de brute force.
        user.LastLogin = DateTime.UtcNow;
        _ctx.Users.Update(user);
        _ctx.SaveChanges();

        if (!IsValidPassword(user, decryptedPass))
        {
            result.Messages.Add("Email ou senha incorretos");
            return result;
        }

        if (!user.Active)
        {
            result.Messages.Add("Usuário com acesso temporariamente suspenso.");
            return result;
        }

        result.Value = UserCleanup(user);
        return result;
    }

    public async Task<Result<User>> AuthenticationByGoogleIdToken(string idToken)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        }
        catch (InvalidJwtException)
        {
            throw new BizException(BizException.Error.NotAuthorized, "Token inválido.");
        }

        var user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user == null)
        {
            user = new User
            {
                Name = payload.Name,
                Email = payload.Email,
                IsEmailVerified = true,
                IsPhoneVerified = false,
                AllowSendingEmail = true,
                Password = string.Empty,
            };

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();
        }

        if (user.IsBruteForceLogin())
            throw new BizException(BizException.Error.NotAuthorized, "Login bloqueado por 30 segundos, para proteger sua conta.");

        user.LastLogin = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();

        var result = Validate(user, x => x.Email);

        if (!user.Active)
        {
            result.Messages.Add("Usuário com acesso temporariamente suspenso.");
            return result;
        }

        result.Value = UserCleanup(user);
        return result;

    }

    public override Result<User> Insert(User user)
    {
        var result = Validate(user);
        if (!result.Success) return result;

        if (string.IsNullOrEmpty(user.Password))
            throw new BizException("Campo senha é obrigatório.");

        user.CreatedAt = DateTime.UtcNow;
        user.Email = user.Email.ToLowerInvariant();
        user = GetUserEncryptedPass(user);

        if (_entity.Any(x => x.Email == user.Email))
            throw new BizException("Esse email já existe no sistema.");
        
        _entity.Add(user);
        _ctx.SaveChanges();

        result.Value = user;
        return result;
    }

    public override Result<User> Update(User user)
    {
        Result<User> result = Validate(user, x =>
           x.Email,
            x => x.Linkedin,
            x => x.Name,
            x => x.Phone,
            x => x.Id);

        if (!result.Success) return result;

        var userAux = _entity.Where(u => u.Id == user.Id).FirstOrDefault();

        if (userAux == null) result.Messages.Add("Usuário não existe.");

        if (_entity.Any(u => u.Email == user.Email && u.Id != user.Id))
            result.Messages.Add("Email já existe.");

        if (result.Success)
        {
            // o mapper ignora campos como password e lastLogin.
            _mapper.Map(user, userAux);

            _entity.Update(userAux);
            _ctx.SaveChanges();

            result.Value = UserCleanup(userAux);
        }

        return result;
    }

    public User FindById(Guid IdUser, Guid IdAdmin)
    {
        var user = _entity.Find(IdUser);
        if (user == null) throw new BizException(BizException.Error.NotFound, "Usuário não encontrado.");

        AddAccessHistory(IdUser, IdAdmin);
        return user;
    }

    private void AddAccessHistory(Guid IdUser, Guid IdAdmin)
    {
        var admin = FindById(IdAdmin);

        var accessHistory = new AccessHistory
        {
            UserId = IdUser,
            VisitorName = admin.Name,
            Profile = admin.GetLgpdProfile()
        };

        _ctx.AccessHistories.Add(accessHistory);
        _ctx.SaveChanges();
    }

    public Result<User> ValidOldPasswordAndChangeUserPassword(User user, string newPassword)
    {
        var resultUserAuth = this.AuthenticationByIdAndPassword(user);

        if (resultUserAuth.Success)
            ChangeUserPassword(resultUserAuth.Value, newPassword);

        return resultUserAuth;
    }

    public Result<User> ChangeUserPassword(User user, string newPassword)
    {
        // Senha forte não é mais obrigatória. Apenas validação de tamanho.
        if (newPassword.Length < 6 || newPassword.Length > 32)
            throw new BizException("A senha deve ter entre 6 e 32 letras.");

        user.ChangePassword(newPassword);
        user = GetUserEncryptedPass(user);

        _entity.Update(user);
        _ctx.SaveChanges();

        var result = new Result<User>(user);

        return result;
    }

    public Result GenerateHashCodePasswordAndSendEmailToUser(string email)
    {
        var result = new Result();
        var user = _entity.FirstOrDefault(e => e.Email == email);

        if (user == null)
        {
            result.Messages.Add("E-mail não encontrado.");
            return result;
        }

        user.GenerateHashCodePassword();
        
        _entity.Update(user);
        _ctx.SaveChanges();

        _userEmailService.SendEmailForgotMyPasswordToUserAsync(user).Wait();
        result.SuccessMessage = "E-mail enviado com as instruções para recuperação da senha.";
        return result;
    }

    public Result ConfirmHashCodePassword(string hashCodePassword)
    {
        var result = new Result();

        var userConfirmedHashCodePassword = _entity.FirstOrDefault(e => e.HashCodePassword.Equals(hashCodePassword));

        if (userConfirmedHashCodePassword == null)
            result.Messages.Add("Hash code não encontrado.");
        else if (result.Success && !userConfirmedHashCodePassword.HashCodePasswordIsValid(hashCodePassword))
            result.Messages.Add("Chave errada ou expirada. Por favor gere outra chave");
        else
            result.Value = UserCleanup(userConfirmedHashCodePassword);

        return result;
    }

    public async Task<UserEmailAndPhoneVerifiedDto> IsEmailAndPhoneVerified(string email, string phone)
    {
        var isEmailVerified = false;
        var isPhoneVerified = false;

        var userEmail = await _ctx.Users.FirstOrDefaultAsync(a => a.Email == email);
        var userPhone = await _ctx.Users.FirstOrDefaultAsync(a => a.Phone == phone);

        if (userEmail != null) isEmailVerified = userEmail.IsEmailVerified;
        if (userPhone != null) isPhoneVerified = userPhone.IsPhoneVerified;

        return new UserEmailAndPhoneVerifiedDto
        {
            IsEmailVerified = isEmailVerified,
            IsPhoneVerified = isPhoneVerified
        };
    }

    public async Task SendEmailVerificationCode(string email)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            throw new BizException(BizException.Error.NotFound, $"Não encontrei nenhum email no sistema igual a '{email}'.");

        if (user.IsEmailVerified)
            throw new BizException(BizException.Error.BadRequest, $"O email '{email}' já estava verificado.");

        user.EmailVerificationCode = GetRandomVerificationCode();
        await _ctx.SaveChangesAsync();

        await _userEmailService.SendEmailVerificationCode(user);
    }

    public async Task SendPhoneVerificationCode(string phone)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Phone == phone);

        if (user == null)
            throw new BizException(BizException.Error.NotFound, $"Não encontrei nenhum Phone no sistema igual a '{phone}'.");

        if (user.IsPhoneVerified)
            throw new BizException(BizException.Error.BadRequest, $"O Phone '{phone}' já estava verificado.");

        user.PhoneVerificationCode = GetRandomVerificationCode();
        await _ctx.SaveChangesAsync();

        await _smsService.SendMessage(user.Phone, $"Salve Elas app. Segue o código para você confirmar seu Phone: {user.PhoneVerificationCode}");
    }

    public async Task<User> EmailVerification(string email, string verificationCode)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            throw new BizException(BizException.Error.NotFound, $"Não encontrei nenhum email no sistema igual a '{email}'.");

        if (user.IsEmailVerified)
            throw new BizException(BizException.Error.BadRequest, $"O email '{email}' já estava verificado.");

        if (user.EmailVerificationCode != verificationCode)
            throw new BizException(BizException.Error.BadRequest, $"O código de verificação está errado.");

        user.IsEmailVerified = true;
        await _ctx.SaveChangesAsync();

        return user;
    }

    public async Task<User> PhoneVerification(string phone, string verificationCode)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Phone == phone);

        if (user == null)
            throw new BizException(BizException.Error.NotFound, $"Não encontrei nenhum Phone no sistema igual a '{phone}'.");

        if (user.IsPhoneVerified)
            throw new BizException(BizException.Error.BadRequest, $"O Phone '{phone}' já estava verificado.");

        if (user.PhoneVerificationCode != verificationCode)
            throw new BizException(BizException.Error.BadRequest, $"O código de verificação está errado.");

        user.IsPhoneVerified = true;
        await _ctx.SaveChangesAsync();

        return user;
    }
    public bool IsValidPassword(User user, string decryptedPass)
    {
        return user.Password == Hash.Create(decryptedPass, user.PasswordSalt);
    }


    #endregion Public

    #region Private

    private string GetRandomVerificationCode()
    {
        Random random = new Random();
        var result = "";

        for (int i = 0; i < 4; i++)
        {
            double numeroAleatorio = random.NextDouble();
            int valorInteiro = (int)(numeroAleatorio * 10);
            result += valorInteiro.ToString();
        }

        return result;
    }

    private Result<User> AuthenticationByIdAndPassword(User user)
    {
        var result = Validate(user, x => x.Id, x => x.Password);

        string decryptedPass = user.Password;

        user = _entity
            .Where(e => e.Id == user.Id)
            .FirstOrDefault();

        if (user == null || !IsValidPassword(user, decryptedPass))
        {
            result.Messages.Add("Senha incorreta");
            return result;
        }

        result.Value = UserCleanup(user);
        return result;
    }

    private User GetUserEncryptedPass(User user)
    {
        user.PasswordSalt = Salt.Create();
        user.Password = Hash.Create(user.Password, user.PasswordSalt);
        return user;
    }

    private User UserCleanup(User user)
    {
        user.Password = string.Empty;
        user.PasswordSalt = string.Empty;
        return user;
    }

    





    #endregion Private
}