using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

internal sealed class DeleteHardSkillRequest : ICommandHandler<DeleteHardSkillRequest.Command, HardSkillDto>
{
    private readonly ITeamBoardDbContext _dbContext;

    public DeleteHardSkillRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HardSkillDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var canHardSkillBeDeleted = await CanHardSkillBeDeleted(request.Id, cancellationToken);

        if (!canHardSkillBeDeleted)
        {
            throw new EntityConflictException("You cannot leave a specialization that is already assigned to users without hard skills");
        }

        var hardSkill = await _dbContext.HardSkills.FindAsync([request.Id], cancellationToken);

        hardSkill!.IsActive = false;

        await _dbContext.SpecializationTemplateHardSkills
            .Where(x => x.HardSkillId == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new HardSkillDto
        {
            Id = hardSkill.Id,
            Name = hardSkill.Name,
            HardSkillGroupId = hardSkill.HardSkillGroupId
        };
    }

    private async Task<bool> CanHardSkillBeDeleted(int skillId, CancellationToken cancellationToken)
    {
        return !await _dbContext.UserSpecializations
            .Where(us => us.SpecializationTemplate!.SpecializationTemplateHardSkills
                .Any(x => x.HardSkillId == skillId && x.HardSkill!.IsActive))
            .GroupBy(us => us.SpecializationTemplateId)
            .Where(g => g.SelectMany(us => us.SpecializationTemplate!.SpecializationTemplateHardSkills)
                .Count(x => x.HardSkill!.IsActive) == 1)
            .AnyAsync(cancellationToken);
    }

    internal sealed record Command(int Id) : ICommand<HardSkillDto>, ITransactionalRequest;
}
