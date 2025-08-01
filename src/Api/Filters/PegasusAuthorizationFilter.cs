using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MockExams.Api.Filters;

public class PegasusAuthorizationFilter : ActionFilterAttribute
{
    private readonly string[] _allowedRoles;
    private readonly ILogger<PegasusAuthorizationFilter>? _logger;

    public PegasusAuthorizationFilter(string[] allowedRoles, ILogger<PegasusAuthorizationFilter>? logger = null)
    {
        _allowedRoles = allowedRoles;
        _logger = logger;
    }

    public PegasusAuthorizationFilter(params string[] allowedRoles)
        : this(allowedRoles, null) { }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var pegasusRole = user.Claims.FirstOrDefault(c => c.Type == "pegasusRole")?.Value;
        var userId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (pegasusRole == null || !_allowedRoles.Contains(pegasusRole))
        {
            _logger?.LogWarning("Acesso negado. UserId: {UserId}, Role: {Role}, Endpoint: {Endpoint}",
                userId ?? "desconhecido",
                pegasusRole ?? "não informado",
                context.ActionDescriptor.DisplayName);

            throw new BizException(BizException.Error.Forbidden);
        }

        base.OnActionExecuting(context);
    }
}
