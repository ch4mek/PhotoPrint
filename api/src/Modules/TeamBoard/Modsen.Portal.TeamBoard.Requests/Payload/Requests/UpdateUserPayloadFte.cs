using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Payload.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Requests;

internal sealed class UpdateUserPayloadFte : ICommandHandler<UpdateUserPayloadFte.Command, PayloadUserFteRequestDto>{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _validateUserBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly int _averageValueForFutureMonths = 168;

    public UpdateUserPayloadFte(
        ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule validateUserBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _validateUserBusinessRule = validateUserBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PayloadUserFteRequestDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var canBeRetrieved = await _validateUserBusinessRule.IsCorrespondsToBusiness(request.UserId,
            cancellationToken);

        if (!canBeRetrieved ||
            _authenticatedUserContext.GetRequiredUserId() == request.UserId)
        {
            throw new ActionNotAllowedException();
        }

        if (request.Fte < 0 || request.Fte > 9.99m)
        {
            throw new InvalidDataAppException("Limit for fte from 0 to 9.99");
        }

        var userPayloadFte = await _teamBoardDbContext.EstimatedUserMonthPayloads
            .Where(x => x.UserId == request.UserId)
            .Where(x => x.PayloadYear == request.Year)
            .Where(x => x.PayloadMonthOfYear == request.Month)
            .FirstOrDefaultAsync(cancellationToken);


        var user = await _teamBoardDbContext.TeamBoardUsers.AsNoTracking()
            .Where(x=>x.UserId == request.UserId)
            .FirstAsync(cancellationToken);

        if (user.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        var averageValueForFutureMonths = await _teamBoardDbContext.WorkingHoursPerMonths.AsNoTracking()
            .Where(x => x.Year == request.Year)
            .Where(x => x.MonthOfYear == request.Month)
            .Where(x => x.CountryId == user.CountryId)
            .Select(x => x.Hours)
            .FirstOrDefaultAsync(cancellationToken);

        averageValueForFutureMonths = averageValueForFutureMonths == 0 ? _averageValueForFutureMonths : averageValueForFutureMonths;

        if (userPayloadFte != null)
        {
            userPayloadFte.EstimatedFte = request.Fte;
            userPayloadFte.UpdatedById = _authenticatedUserContext.UserId;
        }
        else
        {
            userPayloadFte = new EstimatedUserMonthPayload
            {
                UserId = request.UserId,
                PayloadYear = request.Year,
                PayloadMonthOfYear = request.Month,
                EstimatedFte = request.Fte,
                CreatedById = _authenticatedUserContext.UserId
            };

            _teamBoardDbContext.EstimatedUserMonthPayloads.Add(userPayloadFte);
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return new PayloadUserFteRequestDto(
            userPayloadFte.UserId,
            userPayloadFte.PayloadYear,
            userPayloadFte.PayloadMonthOfYear,
            userPayloadFte.EstimatedFte,
            userPayloadFte.EstimatedFte * averageValueForFutureMonths);
    }

    internal sealed record Command(int UserId, int Year, int Month, decimal Fte) : ICommand<PayloadUserFteRequestDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotNull().NotEmpty();

            RuleFor(x => x.Year).NotNull().NotEmpty();

            RuleFor(x => x.Month).NotNull().NotEmpty();

            RuleFor(x => x.Fte).NotNull();
        }
    }
}
