using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

internal interface ICanUserProjectBeCreatedBusinessRule : IBusinessRuleWithFailReason<CanUserProjectBeCreatedModel>;

internal record CanUserProjectBeCreatedModel(int? Id, int UserId, int ProjectId, DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo);

internal class CanUserProjectBeCreatedBusinessRule : ICanUserProjectBeCreatedBusinessRule
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public CanUserProjectBeCreatedBusinessRule(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(
        CanUserProjectBeCreatedModel? model, CancellationToken cancellationToken)
    {
        var existingUserProjects = await _timeTrackDbContext.ProjectUsers
            .Where(projectUser => projectUser.UserId == model!.UserId && projectUser.ProjectId == model.ProjectId)
            .Select(projectUser => new
            {
                projectUser.Id,
                projectUser.UserOnProjectFrom,
                projectUser.UserOnProjectTo
            })
            .ToListAsync(cancellationToken);

        if (existingUserProjects.Count == 0)
        {
            return BusinessRuleResult.Success();
        }

        if (model!.Id != null)
        {
            existingUserProjects = existingUserProjects.Where(x => x.Id != model.Id).ToList();
        }

        var isMatching = existingUserProjects
            .Any(x =>
            {
                if (model.UserOnProjectTo.HasValue)
                {
                    if (!x.UserOnProjectTo.HasValue)
                    {
                        return x.UserOnProjectFrom <= model.UserOnProjectTo;
                    }
                    else
                    {
                        return !(x.UserOnProjectFrom > model.UserOnProjectTo || x.UserOnProjectTo < model.UserOnProjectFrom);
                    }
                }
                else
                {
                    if (x.UserOnProjectTo.HasValue)
                    {
                        return x.UserOnProjectTo >= model.UserOnProjectFrom;
                    }

                    return true;
                }
            });

        return isMatching
            ? BusinessRuleResult.Fail(PortalMessages.ProjectAssignmentForPeriodAlreadyExists)
            : BusinessRuleResult.Success();
    }
}
