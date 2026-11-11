using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal class ProjectUserUpdatedEventConsumer : IConsumer<ProjectUserUpdatedEvent>
{
    private readonly IFinanceDbContext _dbContext;
    private readonly IConnectionFactory _connectionFactory;

    public ProjectUserUpdatedEventConsumer(IFinanceDbContext dbContext, IConnectionFactory connectionFactory)
    {
        _dbContext = dbContext;
        _connectionFactory = connectionFactory;
    }

    public async Task Consume(ConsumeContext<ProjectUserUpdatedEvent> context)
    {
        var @event = context.Message;

        if (@event.UserPosition.IsIntern())
        {
            return;
        }

        var projectUser = await _dbContext.FinanceProjectUsers
            .FirstAsync(pu => pu.Id == @event.ProjectUserId, context.CancellationToken);

        var prevUserOnProjectFrom = projectUser.UserOnProjectFrom;
        var prevUserOnProjectTo = projectUser.UserOnProjectTo;

        projectUser.UserId = @event.UserId;
        projectUser.ProjectId = @event.ProjectId;
        projectUser.UserOnProjectFrom = @event.UserOnProjectFrom;
        projectUser.UserOnProjectTo = @event.UserOnProjectTo;
        projectUser.IsTimeOnProjectBillable = @event.IsTimeOnProjectBillable;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await SynchronizeRatesWithAssignments(@event.UserId, @event.ProjectId,
            prevUserOnProjectFrom, prevUserOnProjectTo,
            @event.UserOnProjectFrom, @event.UserOnProjectTo, context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private async Task SynchronizeRatesWithAssignments(int userId, int projectId,
        DateOnly prevUserOnProjectFrom, DateOnly? prevUserOnProjectTo,
        DateOnly updatedUserOnProjectFrom, DateOnly? updatedUserOnProjectTo, CancellationToken cancellationToken)
    {
        var prevMonthFrom = new DateOnly(prevUserOnProjectFrom.Year, prevUserOnProjectFrom.Month, 1);
        var updatedMonthFrom = new DateOnly(updatedUserOnProjectFrom.Year, updatedUserOnProjectFrom.Month, 1);

        if (prevMonthFrom < updatedMonthFrom)
        {
            await DeleteRates(userId, projectId, cancellationToken);
            return;
        }

        if (updatedUserOnProjectTo is not null)
        {
            if (prevUserOnProjectTo is null)
            {
                await DeleteRates(userId, projectId, cancellationToken);
                return;
            }

            var currenMonthTo = new DateOnly(prevUserOnProjectTo!.Value.Year, prevUserOnProjectTo!.Value.Month, 1);
            var updatedMonthTo = new DateOnly(updatedUserOnProjectTo.Value.Year, updatedUserOnProjectTo.Value.Month, 1);

            if (currenMonthTo > updatedMonthTo)
            {
                await DeleteRates(userId, projectId, cancellationToken);
                return;
            }            
        }
    }

    private async Task DeleteRates(int userId, int projectId, CancellationToken cancellationToken)
    {
        var assignDates = await _dbContext.FinanceProjectUsers
            .Where(pu => pu.UserId == userId && pu.ProjectId == projectId)
            .OrderBy(pu => pu.UserOnProjectFrom)
            .Select(pu => new AssignDatesModel(pu.UserOnProjectFrom, pu.UserOnProjectTo))
            .ToListAsync(cancellationToken);

        var rates = await _dbContext.ProjectRates
            .Where(r => r.UserId == userId && r.ProjectId == projectId)
            .OrderBy(r => r.RateFrom)
            .ToListAsync(cancellationToken);

        var assignIndex = 0;
        var validRates = new List<ProjectRateEntity>();

        foreach (var rate in rates)
        {
            var isValid = IsRateWithinAssigns(ref assignIndex, assignDates, rate);

            if (isValid)
            {
                validRates.Add(rate);
            }
            else
            {
                rate.IsDeleted = true;
            }
        }

        for (var i = 0; i < validRates.Count; i++)
        {
            validRates[i].RateTo =
                i == validRates.Count - 1
                    ? null
                    : validRates[i + 1].RateFrom.AddDays(-1);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static bool IsRateWithinAssigns(ref int assignIndex, List<AssignDatesModel> assignDates, ProjectRateEntity rate)
    {
        while (assignIndex < assignDates.Count)
        {
            var assign = assignDates[assignIndex];

            if (rate.RateFrom < assign.UserOnProjectFrom)
            {
                break;
            }

            if (assign.UserOnProjectTo.HasValue && rate.RateFrom > assign.UserOnProjectTo.Value)
            {
                assignIndex++;
                continue;
            }

            break;
        }

        var isValid = assignIndex < assignDates.Count
            && rate.RateFrom >= assignDates[assignIndex].UserOnProjectFrom
            && (!assignDates[assignIndex].UserOnProjectTo.HasValue
                || rate.RateFrom <= assignDates[assignIndex].UserOnProjectTo!.Value);

        return isValid;
    }

    private sealed record AssignDatesModel(DateOnly UserOnProjectFrom, DateOnly? UserOnProjectTo);
      
    internal sealed class ProjectUserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ProjectUserUpdatedEventConsumer>(
            "project-user-updated-finance-queue",
            "project-user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ProjectUserUpdatedEventConsumer,
        ProjectUserUpdatedEventConsumerDefinition>;
}
