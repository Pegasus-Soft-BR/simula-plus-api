using Domain.Common;
using Domain.Enums;
using System;
using System.Text.RegularExpressions;

namespace Domain;

public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public string Linkedin { get; set; }
    public  string Phone{ get; set; }
    public Profile Profile { get;  set; } = Profile.Usuario;
    public bool Active { get; set; } = true;
    public bool AllowSendingEmail { get; set; } = true;



    // esqueci minha senha
    public string HashCodePassword { get; set; }
    public DateTime HashCodePasswordExpiryDate { get; set; } = DateTime.MinValue;

    // evitar ataques de força bruta
    public DateTime LastLogin { get; set; } = DateTime.MinValue;


    // verificação de email e phone
    public bool IsEmailVerified { get; set; } = false;
    public bool IsPhoneVerified { get; set; } = false;
    public string EmailVerificationCode { get; set; }
    public string PhoneVerificationCode { get; set; }


    public bool PasswordIsStrong()
    {
        Regex rgx = new Regex(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9])[A-Za-z0-9\d$@$!%_*_?&#.,-_:;]{8,}");
        if (string.IsNullOrEmpty(Password) || !rgx.IsMatch(Password)) return false;

        return true;
    }

    public User Cleanup()
    {
        this.Password = string.Empty;
        this.PasswordSalt = string.Empty;
        return this;
    }

    public void GenerateHashCodePassword()
    {
        this.HashCodePassword =  Guid.NewGuid().ToString();
        this.HashCodePasswordExpiryDate = DateTime.UtcNow.AddDays(1); 
    }

    public bool HashCodePasswordIsValid(string hashCodePassword)
         => hashCodePassword == this.HashCodePassword 
            && (this.HashCodePasswordExpiryDate.Date == DateTime.UtcNow.AddDays(1).Date
               || this.HashCodePasswordExpiryDate.Date == DateTime.UtcNow.Date);


    public void ChangePassword(string password)
    {
        this.Password = password;
    }

    public bool IsBruteForceLogin()
    {
        var refDate = DateTime.UtcNow.AddSeconds(-30);
        return LastLogin > refDate;
    }

    public void Anonymize()
    {
        Name = "USUÁRIO ANONIMIZADO";
        Email = "anonimizado_" + DateTime.Now.ToFileTime() + "@MockExams.com.br";
        Active = false;
        AllowSendingEmail = false;
        Linkedin = null;
        Phone = null;

    }

    // Para uso no LGPD
    public string GetLgpdProfile()
    {
        return Profile.ToString();
    }

}
