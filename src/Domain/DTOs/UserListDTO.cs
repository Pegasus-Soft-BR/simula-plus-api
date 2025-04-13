using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.DTOs;

public class UserListDTO
{

    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public Profile Profile { get; set; }


}
