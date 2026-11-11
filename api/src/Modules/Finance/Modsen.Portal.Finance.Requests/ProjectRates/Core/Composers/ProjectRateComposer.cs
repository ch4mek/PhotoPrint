using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.Composers;

internal interface IProjectRateComposer
{
    Task<ProjectRateCoreDto> ComposeAsync(CreateProjectRateCoreDto request, CancellationToken cancellationToken);
}

internal class ProjectRateComposer : IProjectRateComposer
{
    private readonly IProjectRateEncryptor _projectRateEncryptor;
    private readonly IFinanceDbContext _dbContext;

    public ProjectRateComposer(IProjectRateEncryptor projectRateEncryptor, IFinanceDbContext dbContext)
    {
        _projectRateEncryptor = projectRateEncryptor;
        _dbContext = dbContext;
    }

    public async Task<ProjectRateCoreDto> ComposeAsync(CreateProjectRateCoreDto request, CancellationToken cancellationToken)
    {
        var newRate = new ProjectRateEntity
        {
            UserId = request.UserId,
            ProjectId = request.ProjectId,
            HourlyRate = _projectRateEncryptor.Encrypt(request.HourlyRate)!,
            CurrencyId = request.CurrencyId,
            RateFrom = request.RateFrom,
        };

        var previousRate = await _dbContext.ProjectRates.Where(r => r.UserId == request.UserId && r.ProjectId == request.ProjectId)
            .Where(r => r.RateFrom < request.RateFrom)
            .OrderByDescending(r => r.RateFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (previousRate != null)
        {
            previousRate.RateTo = request.RateFrom.AddDays(-1);
        }

        var nextRate = await _dbContext.ProjectRates.Where(r => r.UserId == request.UserId && r.ProjectId == request.ProjectId)
            .Where(r => r.RateFrom > request.RateFrom)
            .OrderBy(r => r.RateFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (nextRate != null)
        {
            newRate.RateTo = nextRate.RateFrom.AddDays(-1);
        }

        var entity = await _dbContext.ProjectRates.AddAsync(newRate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var createdRate = await _dbContext.ProjectRates
            .Include(r => r.User)
            .Include(r => r.Project)
            .ThenInclude(p => p!.Client)
            .FirstAsync(r => r.Id == entity.Entity.Id, cancellationToken);

        return new ProjectRateCoreDto
        {
            Id = createdRate.Id,
            UserId = createdRate.UserId,
            ProjectId = createdRate.ProjectId,
            HourlyRate = _projectRateEncryptor.Decrypt(createdRate.HourlyRate)!,
            CurrencyId = createdRate.CurrencyId,
            RateFrom = createdRate.RateFrom,
            User = FinanceUserCoreDto.Of(createdRate.User),
            Project = FinanceProjectCoreDto.Of(createdRate.Project),
        };
    }
}
