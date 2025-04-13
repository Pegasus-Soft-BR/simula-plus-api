using System.Collections.Generic;

namespace MockExams.Service.Authorization;

public class Permissions
{
    public enum Permission
    {
        Admin, // acesso total.
        Usuario, // usuario final.
    }

    public static List<Permission> AdminPermissions { get; } = new List<Permission>() { Permission.Admin, Permission.Usuario };
    public static List<Permission> UsuarioPermissions { get; } = new List<Permission>() { Permission.Usuario };
}
