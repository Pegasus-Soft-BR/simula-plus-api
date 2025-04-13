using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs;

public class UserDto
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    public string Linkedin { get; set; }


    public string Phone { get; set; }

    public bool AllowSendingEmail { get; set; }

    public DateTime BirthDate { get; set; }




    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
}
