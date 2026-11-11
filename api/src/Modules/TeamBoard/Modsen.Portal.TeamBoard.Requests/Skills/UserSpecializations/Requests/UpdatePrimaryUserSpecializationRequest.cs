using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;

internal sealed class UpdatePrimaryUserSpecializationRequest
    : ICommandHandler<UpdatePrimaryUserSpecializationRequest.Command, UserSpecializationCoreDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IUserSpecializationLoader _userSpecializationLoader;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

    public UpdatePrimaryUserSpecializationRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IUserSpecializationLoader userSpecializationLoader,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _userSpecializationLoader = userSpecializationLoader;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<UserSpecializationCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var canTeamBoardUserBeRetrieved =
            await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId, AccessOptions.Manager, cancellationToken);
        if (!canTeamBoardUserBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var oldPrimarySpecialization = await GetUserPrimarySpecializationByUserId(request.UserId, cancellationToken);

        if (oldPrimarySpecialization == null)
        {
            throw new EntityNotFoundException("User primary specialization was not found");
        }

        var isNewSpecializationTemplateEqualsToOld = request.NewSpecializationTemplateId ==
            oldPrimarySpecialization.SpecializationTemplateId;
        if (isNewSpecializationTemplateEqualsToOld)
        {
            return (await _userSpecializationLoader.Load(oldPrimarySpecialization.Id, cancellationToken))!;
        }

        var isUserHasSpecializationTemplate = await IsUserAlreadyHasSpecializationTemplate(
            request, cancellationToken);

        if (!isUserHasSpecializationTemplate)
        {
            throw new InvalidDataAppException(
                $"User has no specialization with id {request.NewSpecializationTemplateId}");
        }

        oldPrimarySpecialization.IsPrimary = false;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var createdUserSpecialization = await SetNewPrimarySpecialization(request, cancellationToken);

        return (await _userSpecializationLoader.Load(createdUserSpecialization.Id, cancellationToken))!;
    }

    private async Task<UserSpecialization> SetNewPrimarySpecialization(Command request, CancellationToken token)
    {
        var newPrimarySpecialization = await _teamBoardDbContext.UserSpecializations
            .Where(x =>
                x.UserId == request.UserId &&
                x.SpecializationTemplateId == request.NewSpecializationTemplateId &&
                !x.IsPrimary)
            .FirstOrDefaultAsync(token);

        newPrimarySpecialization!.IsPrimary = true;

        await _teamBoardDbContext.SaveChangesAsync(token);

        return newPrimarySpecialization;
    }

    private async Task<UserSpecialization?> GetUserPrimarySpecializationByUserId(
        int userId, CancellationToken token)
    {
        return await _teamBoardDbContext.UserSpecializations
            .Where(specialization => specialization.UserId == userId && specialization.IsPrimary)
            .FirstOrDefaultAsync(token);
    }

    private async Task<bool> IsUserAlreadyHasSpecializationTemplate(Command request, CancellationToken token)
    {
        return await _teamBoardDbContext.UserSpecializations
            .AnyAsync(us =>
                us.SpecializationTemplateId == request.NewSpecializationTemplateId &&
                us.UserId == request.UserId &&
                !us.IsPrimary,
                token);
    }

    internal sealed record Command(int UserId, int NewSpecializationTemplateId) :
        ICommand<UserSpecializationCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.NewSpecializationTemplateId)
                .NotNull()
                .NotEmpty();

        }
    }
}
