using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Attendance.Requests.Attendances.Core.Calculators;
using Modsen.Portal.Attendance.Requests.Consumers;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Attendance.Requests;

public sealed class AttendanceRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IAttendanceTotalCalculator, AttendanceTotalCalculator>();
    }

    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupCreatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupUpdatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupSyncEventConsumer.RegistrationDefinition();
    }
}
