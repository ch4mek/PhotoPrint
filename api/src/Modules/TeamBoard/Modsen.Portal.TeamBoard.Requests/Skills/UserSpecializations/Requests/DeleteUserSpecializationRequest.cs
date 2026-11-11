using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;

internal sealed class DeleteUserSpecializationRequest : ICommandHandler<DeleteUserSpecializationRequest.Command, int>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

    public DeleteUserSpecializationRequest(
        ITeamBoardDbContext teamBoardDbContext, ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var userSpecialization = await _teamBoardDbContext.UserSpecializations
            .Where(specialization => specialization.Id == request.Id)
            .Include(userSpecialization => userSpecialization.SpecializationHardSkills)
            .FirstOrDefaultAsync(cancellationToken);

        if (userSpecialization == null)
        {
            throw new EntityNotFoundException($"User specialization with id {request.Id} was not found");
        }

        if (userSpecialization.IsPrimary)
        {
            throw new InvalidDataAppException("User's primary specialization cannot be deleted");
        }

        var canTeamBoardUserBeRetrieved =
            await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(userSpecialization.UserId, AccessOptions.Manager, cancellationToken);
        if (!canTeamBoardUserBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var snapshots = await GetSnapshotSpecializations(
            userSpecialization.UserId,
            userSpecialization.SpecializationTemplateId,
            cancellationToken);

        foreach (var snapshot in snapshots)
        {
            _teamBoardDbContext.UserSkillsSnapshotSpecializationHardSkills.RemoveRange(snapshot.HardSkills);
        }

        _teamBoardDbContext.UserSkillsSnapshotSpecializations.RemoveRange(snapshots);

        _teamBoardDbContext.UserSpecializationHardSkills.RemoveRange(userSpecialization.SpecializationHardSkills);
        _teamBoardDbContext.UserSpecializations.Remove(userSpecialization);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return userSpecialization.Id;
    }

    private Task<List<UserSkillsSnapshotSpecialization>> GetSnapshotSpecializations(int userId, int templateId,
        CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.UserSkillsSnapshotSpecializations
            .Where(snapshotSpecialization => snapshotSpecialization.UserSkillsSnapshot!.UserId == userId)
            .Where(snapshotSpecialization => snapshotSpecialization.SpecializationTemplateId == templateId)
            .Include(snapshotSpecialization => snapshotSpecialization.HardSkills)
            .ToListAsync(cancellationToken);
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
