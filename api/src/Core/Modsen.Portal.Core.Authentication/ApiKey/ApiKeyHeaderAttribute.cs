using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.Authentication.ApiKey;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ApiKeyHeaderAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var keyValidator = context.HttpContext.RequestServices.GetRequiredService<IApiKeyValidator>();
        if (!keyValidator.IsValidProvidedKey())
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            return;
        }

        base.OnActionExecuting(context);
    }
}
