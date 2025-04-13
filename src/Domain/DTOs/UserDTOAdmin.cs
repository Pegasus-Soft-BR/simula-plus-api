using Domain.Enums;

namespace Domain.DTOs;

public class UserDtoAdmin : UserDto
{
    public Profile Profile { get; set; } = Profile.Usuario;
    public bool Active { get; set; } = true;
    public string Password { get; set; }
}
