using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.Authentication.PayslipsKey;

[AttributeUsage(AttributeTargets.Class)]
public sealed class PayslipsApiKeyHeaderAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var keyValidator = context.HttpContext.RequestServices.GetRequiredService<IPayslipsKeyValidator>();
        if (!keyValidator.IsValidProvidedKey())
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}
