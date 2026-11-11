using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Cv.Requests.Cvs.Core.BusinessRules;
using Modsen.Portal.Cv.Requests.Cvs.Core.DbQueries;
using Modsen.Portal.Cv.Requests.Cvs.Core.Validators;

namespace Modsen.Portal.Cv.Requests.Cvs;

public sealed class CvsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICvNameUniqueWithinUserBusinessRule, CvNameUniqueWithinUserBusinessRule>();

        services.AddScoped<ICreateCvValidator, CreateCvValidator>();
        services.AddScoped<IUpdateCvValidator, UpdateCvValidator>();

        services.AddScoped<IGetCvByIdWithIncludesDbQuery, GetCvByIdWithIncludesDbQuery>();
    }
}
