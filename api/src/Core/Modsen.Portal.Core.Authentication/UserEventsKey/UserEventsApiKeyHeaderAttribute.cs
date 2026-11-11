using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.Authentication.UserEventsKey;

[AttributeUsage(AttributeTargets.Class)]
public sealed class UserEventsApiKeyHeaderAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var keyValidator = context.HttpContext.RequestServices.GetRequiredService<IUserEventsKeyValidator>();
        if (!keyValidator.IsValidProvidedKey())
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}
