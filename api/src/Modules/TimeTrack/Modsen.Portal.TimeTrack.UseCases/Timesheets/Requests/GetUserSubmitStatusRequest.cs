using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class GetUserSubmitStatusRequest
    : IQueryHandler<GetUserSubmitStatusRequest.Query, TimesheetSubmitStatusType?>
{
    private readonly IGetTimesheetSubmitStatusDbQuery _getTimesheetSubmitStatusDbQuery;
    private readonly ICanGetSubmitStatusBusinessRule _canGetStatusBusinessRule;

    public GetUserSubmitStatusRequest(ICanGetSubmitStatusBusinessRule canGetStatusBusinessRule,
        IGetTimesheetSubmitStatusDbQuery getTimesheetSubmitStatusDbQuery)
    {
        _getTimesheetSubmitStatusDbQuery = getTimesheetSubmitStatusDbQuery;
        _canGetStatusBusinessRule = canGetStatusBusinessRule;
    }

    public async Task<TimesheetSubmitStatusType?> Handle(Query request, CancellationToken cancellationToken)
    {
        var canGetStatus = await _canGetStatusBusinessRule.IsCorrespondsToBusiness(request.UserId, cancellationToken);

        if (!canGetStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotGetTimesheetSubmitStatusForUser);
        }

        var status = await _getTimesheetSubmitStatusDbQuery.ExecuteAsync(request.UserId,
            new DateOnly(request.Year, request.Month, 1),
            cancellationToken);

        return status;
    }

    internal sealed record Query(int UserId, int Month, int Year) : IQuery<TimesheetSubmitStatusType?>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.UserId)
                .NotNull()
                .NotEmpty()
                .WithMessage(PortalMessages.UserIdHasNotBeenProvided);

            RuleFor(query => query.Month)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(12)
                .WithMessage(PortalMessages.MonthMustBeBetweenOneAndTwelve);

            RuleFor(query => query.Year)
                .GreaterThanOrEqualTo(2020)
                .WithMessage(PortalMessages.CannotUpdateStatusForYearEarlierThan2020);
        }
    }
}
