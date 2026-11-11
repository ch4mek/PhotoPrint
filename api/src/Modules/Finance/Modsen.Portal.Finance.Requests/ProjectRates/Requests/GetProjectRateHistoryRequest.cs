using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Requests;

internal class GetProjectRateHistoryRequest : IQueryHandler<GetProjectRateHistoryRequest.Query, ProjectRateHistoryCoreDto>
{
    private readonly IProjectRateEncryptor _projectRateEncryptor;
    private readonly IFinanceClientsAndUsersQueryBuilder _managedClientsQueryBuilder;
    private readonly ICanFinanceUserEditProjectRatesBusinessRule _canFinanceUserEditProjectRateBusinessRule;
    private readonly IFinanceDbContext _dbContext;

    public GetProjectRateHistoryRequest(
        IProjectRateEncryptor projectRateEncryptor,
        IFinanceClientsAndUsersQueryBuilder managedClientsQueryBuilder,
        ICanFinanceUserEditProjectRatesBusinessRule canFinanceUserEditProjectRateBusinessRule,
        IFinanceDbContext dbContext)
    {
        _projectRateEncryptor = projectRateEncryptor;
        _managedClientsQueryBuilder = managedClientsQueryBuilder;
        _canFinanceUserEditProjectRateBusinessRule = canFinanceUserEditProjectRateBusinessRule;
        _dbContext = dbContext;
    }

    public async Task<ProjectRateHistoryCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.FinanceProjects
                .SingleOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

        var user = await _dbContext.FinanceUsers
                .SingleOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        var managedUserIds = await _managedClientsQueryBuilder.BuildAvailableFinanceUserIdsAsync(cancellationToken);

        var canView = await managedUserIds.ContainsAsync(request.UserId, cancellationToken);

        if (!canView)
        {
            throw new ActionNotAllowedException();
        }

        var rates = await _dbContext.ProjectRates
            .AsNoTracking()
            .Where(r => r.UserId == request.UserId && r.ProjectId == request.ProjectId)
            .OrderBy(r => r.RateFrom)
            .ToListAsync(cancellationToken);

        var keys = rates.ToDictionary(x => x.Id, x => new ProjectRateBusinessKey(x.UserId, x.ProjectId, x.RateFrom));
        var canManage = await _canFinanceUserEditProjectRateBusinessRule
            .IsCorrespondsToBusinessBatch(keys.Values, cancellationToken);

        var canAdd = await _canFinanceUserEditProjectRateBusinessRule
            .IsCorrespondsToBusiness(user.Id, project.Id, DateOnly.MinValue, cancellationToken);

        var result = new ProjectRateHistoryCoreDto
        {
            UserId = request.UserId,
            User = UserCoreDto.Of(user),
            ProjectId = request.ProjectId,
            Project = ProjectCoreDto.Of(project),
            ProjectRates = rates.Select(r => ParseProjectRate(r, canManage[keys[r.Id]])).ToList(),
            CanRateBeAddedByCurrentUser = canAdd
        };

        return result;
    }

    private ProjectRateShortCoreDto ParseProjectRate(ProjectRateEntity entity, bool canManage)
    {
        return new ProjectRateShortCoreDto
        {
            Id = entity.Id,
            HourlyRate = _projectRateEncryptor.Decrypt(entity.HourlyRate),
            CurrencyId = entity.CurrencyId,
            RateFrom = entity.RateFrom,
            CanRateBeManagedByCurrentUser = canManage,
            IsDeleted = entity.IsDeleted,
        };
    }

    internal sealed record Query(int UserId, int ProjectId) : IQuery<ProjectRateHistoryCoreDto>;
}
