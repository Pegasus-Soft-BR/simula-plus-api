using Domain.Common;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Linq;

namespace MockExams.Api.Filters;

public class AppAuthorizationFilter : ActionFilterAttribute
{
    private readonly string[] _allowedRoles;
    private readonly ILogger<AppAuthorizationFilter>? _logger;
    private readonly string _expectedAppSlug;

    public AppAuthorizationFilter(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
        _expectedAppSlug = AppSettings.ServerSettings.AppSlug;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var userId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var appRole = user.Claims.FirstOrDefault(c => c.Type == "appRole")?.Value?.ToLower();
        var appSlug = user.Claims.FirstOrDefault(c => c.Type == "appSlug")?.Value?.ToLower();
        var pegasusRole = user.Claims.FirstOrDefault(c => c.Type == "pegasusRole")?.Value?.ToLower();

        // pegasus admin tem acesso a todos os apps
        if (pegasusRole == "admin")
            return;

        // admin do app tem acesso a tudo do app
        if (appRole == "admin" && appSlug == _expectedAppSlug)
            return;

        // validações normais de acesso.
        if (appSlug == _expectedAppSlug && _allowedRoles.Contains(appRole))
            return;

        Log.Warning("Acesso negado. UserId: {UserId}, AppRole: {AppRole}, AppSlug: {AppSlug}, ExpectedApp: {Expected}, Endpoint: {Endpoint}",
            userId ?? "desconhecido",
            appRole ?? "não informado",
            appSlug ?? "não informado",
            _expectedAppSlug,
            context.ActionDescriptor.DisplayName);

        throw new BizException(BizException.Error.Forbidden);
    }
}
