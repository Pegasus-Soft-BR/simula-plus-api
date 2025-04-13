using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs;

public class EmailVerificationDto
{
    public string Email { get; set; }
    public string VerificationCode { get; set; }
}

public class PhoneVerificationDto
{
    public string Phone { get; set; }
    public string VerificationCode { get; set; }
}
