using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Validators;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates;

public sealed class SpecializationTemplatesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICanUserUpdateSpecializationTemplateBusinessRule,
            CanUserUpdateSpecializationTemplateBusinessRule>();
        services.AddScoped<IIsSpecializationNotAssignedToAnyUserBusinessRule,
            IsSpecializationNotAssignedToAnyUserBusinessRule>();
        services.AddScoped<IIsSpecializationTemplateNameUniqueBusinessRule,
            IsSpecializationTemplateNameUniqueBusinessRule>();
        services.AddScoped<ISpecializationNotAssignedToAnyUserBusinessRule,
            SpecializationNotAssignedToAnyUserBusinessRule>();

        services.AddScoped<ISpecializationTemplateDepartmentsComposer, SpecializationTemplateDepartmentsComposer>();
        services.AddScoped<ISpecializationTemplateHardSkillsComposer, SpecializationTemplateHardSkillsComposer>();

        services.AddScoped<IDeleteSpecializationTemplateHardSkillsDbQuery,
            DeleteSpecializationTemplateHardSkillsDbQuery>();
        services.AddScoped<IDepartmentsSpecializationAssignmentsDbQuery,
            DepartmentsSpecializationAssignmentsDbQuery>();
        services.AddScoped<IGetSpecializationTemplateByIdDbQuery, GetSpecializationTemplateByIdDbQuery>();
        services.AddScoped<IGetSpecializationTemplateDepartmentsDbQuery,
            GetSpecializationTemplateDepartmentsDbQuery>();

        services.AddScoped<IUpdateSpecializationTemplateValidator, UpdateSpecializationTemplateValidator>();
    }
}
