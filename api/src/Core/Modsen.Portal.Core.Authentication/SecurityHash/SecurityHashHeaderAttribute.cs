using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.Authentication.SecurityHash;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SecurityHashHeaderAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var hashValidator = context.HttpContext.RequestServices.GetRequiredService<ISecurityHashValidator>();
        if (!hashValidator.IsValidProvidedHash())
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            return;
        }

        base.OnActionExecuting(context);
    }
}
