using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using TimeTrackUser = Modsen.Portal.TimeTrack.Domain.TimeTrackUser;

namespace Modsen.Portal.TimeTrack.UseCases.Consumers;

internal sealed class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ICreateOrUpdateUserDbQuery _createOrUpdateUserDbQuery;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IConnectionFactory _connectionFactory;

    public UserCreatedEventConsumer(
        ICreateOrUpdateUserDbQuery createOrUpdateUserDbQuery,
        IConnectionFactory connectionFactory,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _createOrUpdateUserDbQuery = createOrUpdateUserDbQuery;
        _connectionFactory = connectionFactory;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;

        var user = new TimeTrackUser
        {
            UserId = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            DepartmentId = @event.DepartmentId,
            ResourceGroupId = @event.ResourceGroupId,
            Email = @event.Email,
            Color = @event.Color,
            AvatarFileId = @event.AvatarFileId,
            IsActive = true,
            CreatedById = @event.CreatedById,
        };

        await _createOrUpdateUserDbQuery.Execute(user, @event.Position, context.CancellationToken);
        await CreateStatusAsync(user, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private async Task CreateStatusAsync(TimeTrackUser user, CancellationToken cancellationToken)
    {
        var startDate = new DateOnly(2020, 1, 1);
        var endDate = DateOnly.FromDateTime(DateTime.Now).AddMonths(-1);

        var ceo = await _timeTrackDbContext.TimeTrackUsers.Where(x => x.Position == PortalRoleNames.Ceo).FirstAsync(cancellationToken);

        for (var date = startDate; date <= endDate; date = date.AddMonths(1))
        {
            var submit = new TimesheetSubmitStatus
            {
                UserId = user.UserId,
                Status = TimesheetSubmitStatusType.Approved,
                DecisionBy = ceo.UserId,
                Date = date,
                Year = date.Year,
                Month = date.Month,
                FactHoursSum = 0,
                BillableHoursSum = 0,
                Snapshot = "[]"
            };

            _timeTrackDbContext.TimesheetSubmitStatuses.Add(submit);
        }

        await _timeTrackDbContext.SaveChanges(cancellationToken);
    }

    internal sealed class UserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserCreatedEventConsumer>("user-created-timetrack-queue", "user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserCreatedEventConsumer,
        UserCreatedEventConsumerDefinition>;
}
