using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;

internal sealed class UpdateUserPlanRequest : ICommandHandler<UpdateUserPlanRequest.Command, string?>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public UpdateUserPlanRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<string?> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == request.UserId && user.IsActive)
            .Select(user => new
            {
                user.ResourceGroupId,
                user.DepartmentId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotChangeUserWithDifferentDepartment);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotChangeUserWithDifferentGroup);
        }

        if (_authenticatedUserContext.UserId == request.UserId)
        {
            throw new ActionNotAllowedException();
        }

        var userPayloadDetails = await _teamBoardDbContext.UserPayloadDetails
            .Where(details => details.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userPayloadDetails is not null)
        {
            userPayloadDetails.Plan = request.Plan;
            userPayloadDetails.UpdatedById = _authenticatedUserContext.UserId;
        }
        else
        {
            userPayloadDetails = new UserPayloadDetails
            {
                UserId = request.UserId,
                Plan = request.Plan,
                CreatedById = _authenticatedUserContext.UserId
            };

            _teamBoardDbContext.UserPayloadDetails.Add(userPayloadDetails);
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return request.Plan;
    }

    internal sealed record Command(int UserId, string? Plan) : ICommand<string?>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Plan)
                .MinimumLength(2)
                .MaximumLength(1000);

            When(x => x.Plan is not null, () =>
            {
                RuleFor(x => x.Plan)
                    .Must(text => !text.IsNullOrEmpty() && !Regex.IsMatch(text, @"^[\s\p{P}|]+$", RegexOptions.None, TimeSpan.FromSeconds(1)))
                    .WithMessage("The string cannot contain only spaces or/and punctuation symbols");
            });
        }
    }
}
