using System;

namespace Domain.DTOs.Commom;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string AppSlug { get; set; }
    public string AppRole { get; set; }

    public string PegasusRole { get; set; }

}

