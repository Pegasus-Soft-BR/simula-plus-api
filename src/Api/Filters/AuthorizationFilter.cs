using Microsoft.AspNetCore.Mvc.Filters;
using Domain.Exceptions;
using MockExams.Service.Authorization;
using System.Linq;
using System.Security.Claims;

namespace MockExams.Api.Filters
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        public Permissions.Permission[] NecessaryPermissions { get; set; }

        public AuthorizationFilter(params Permissions.Permission[] permissions)
        {
            NecessaryPermissions = permissions;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user == null)
                throw new BizException(BizException.Error.NotAuthorized);

            var isAdmin = ((ClaimsIdentity)user.Identity).Claims
                .Any(x => x.Type == ClaimsIdentity.DefaultRoleClaimType.ToString() && x.Value == Domain.Enums.Profile.Admin.ToString());

            var isUsuario = ((ClaimsIdentity)user.Identity).Claims
                .Any(x => x.Type == ClaimsIdentity.DefaultRoleClaimType.ToString() && x.Value == Domain.Enums.Profile.Usuario.ToString());

            // Admin tem acesso a tudo. Nem precisa validar.
            if (isAdmin) return;

            if (isUsuario)
            {
                if (!Permissions.UsuarioPermissions.Any(x => NecessaryPermissions.Contains(x)))
                    throw new BizException(BizException.Error.Forbidden);
            }

            if (!isAdmin && !isUsuario)
            {
                throw new BizException(BizException.Error.Forbidden);
            }

            base.OnActionExecuting(context);
        }
    }
}
