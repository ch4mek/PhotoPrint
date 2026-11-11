using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Requests.Consumers;
using Modsen.Portal.TeamBoard.Requests.Contracts;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Education;
using Modsen.Portal.TeamBoard.Requests.Home;
using Modsen.Portal.TeamBoard.Requests.Interviews;
using Modsen.Portal.TeamBoard.Requests.Payload;
using Modsen.Portal.TeamBoard.Requests.Payslips;
using Modsen.Portal.TeamBoard.Requests.Payslips.Core.Encryptor;
using Modsen.Portal.TeamBoard.Requests.Reviews;
using Modsen.Portal.TeamBoard.Requests.Skills;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.UserSalary;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.UserScores.Core;

namespace Modsen.Portal.TeamBoard.Requests;

public sealed class TeamBoardRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<UserSalaryModule>();
        AddModule<UserSalaryModule>();
        AddModule<SkillsModule>();
        AddModule<PayloadModule>();
        AddModule<EducationModule>();
        AddModule<ContractsModule>();
        AddModule<ReviewsModule>();
        AddModule<PayslipsModule>();
        AddModule<HomeModule>();
        AddModule<InterviewModule>();

        base.Load(services);

        services.AddScoped<ITeamBoardContract, TeamBoardContract>();
        services.AddScoped<IGetProjectUserByIdDbQuery, GetProjectUserByIdDbQuery>();
        services.AddScoped<IGetTeamBoardUsersWithNoIncludesDbQuery, GetTeamBoardUsersWithNoIncludesDbQuery>();

        services.AddScoped<IUpdateProjectUserDbQuery, UpdateProjectUserDbQuery>();
        services.AddScoped<IUpdateTeamBoardProjectDbQuery, UpdateTeamBoardProjectDbQuery>();

        services.AddScoped<IIsTeamBoardUserExistWithAccessControlDbQuery,
                IsTeamBoardUserExistWithAccessControlDbQuery>();
        services.AddScoped<IIsTeamBoardUserExistWithHRAccessControlDbQuery,
            IsTeamBoardUserExistWithHRAccessControlDbQuery>();
        services.AddScoped<IIsFireDateLaterThanHireDateDbQuery, IsFireDateLaterThanHireDateDbQuery>();

        services.AddScoped<ITeamBoardProjectComposer, TeamBoardProjectComposer>();
        services.AddScoped<IProjectUserComposer, ProjectUserComposer>();

        services.AddScoped<IUserSalaryComposer, UserSalaryComposer>();
        services.AddScoped<IUserSalaryLoader, UserSalaryLoader>();

        services.AddScoped<IArePersonalPhoneAndEmailsUniqueBusinessRule, ArePersonalPhoneAndEmailsUniqueBusinessRule>();
        services.AddScoped<IUserPersonalInfoLoader, UserPersonalInfoLoader>();
        services.AddScoped<ICreateOrUpdateUserDbQuery, CreateOrUpdateUserDbQuery>();
        services.AddScoped<IUpdateIsActiveForUserDbQuery, UpdateIsActiveForUserDbQuery>();
        services.AddScoped<IIsTeamBoardUserExistsDbQuery, IsTeamBoardUserExistsDbQuery>();
        services.AddScoped<ISyncDepartmentDbQuery, SyncDepartmentDbQuery>();

        services.AddScoped<IScoreSignatureValidator, ScoreSignatureValidator>();

        AddBusinessRules(services);

        services.AddScoped<IIsTeamBoardUserExistWithAccessControlDbQuery, IsTeamBoardUserExistWithAccessControlDbQuery>();
    }

    public static void AddBusinessRules(IServiceCollection services)
    {
        services.AddScoped<ICanUserUpdateSpecializationTemplateBusinessRule, CanUserUpdateSpecializationTemplateBusinessRule>();

        services.AddScoped<ICanTeamBoardUserBeRetrievedBusinessRule, CanTeamBoardUserBeRetrievedBusinessRule>();
        services.AddScoped<IIsSalaryFromDateValidBusinessRule, IsSalaryFromDateValidBusinessRule>();
        services.AddScoped<IIsSalaryFromDateValidBusinessRule, IsSalaryFromDateValidBusinessRule>();

        services.AddScoped<ITeamBoardPortalSettingsAccessor, TeamBoardPortalSettingsAccessor>();

        services.AddScoped<IPayslipEncryptor, PayslipEncryptor>();
    }

    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new DepartmentSyncEventConsumer.RegistrationDefinition();
        yield return new ProjectCreatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUpdatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUserCreatedEventConsumer.RegistrationDefinition();
        yield return new ProjectUserUpdatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupCreatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupSyncEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupUpdatedEventConsumer.RegistrationDefinition();
        yield return new UserLocationUpdatedEventConsumer.RegistrationDefinition();
        yield return new UserRegisteredLocationUpdatedEventConsumer.RegistrationDefinition();
        yield return new ClientCreatedEventConsumer.RegistrationDefinition();
        yield return new ClientUpdatedEventConsumer.RegistrationDefinition();
    }
}
