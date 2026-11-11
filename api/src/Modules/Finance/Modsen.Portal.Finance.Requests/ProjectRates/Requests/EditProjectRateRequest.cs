using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Loaders;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Requests;

internal sealed class EditProjectRateRequest : ICommandHandler<EditProjectRateRequest.Command, ProjectRateCoreDto>
{
    private readonly IIsProjectRateUniqueForDateBusinessRule _isProjectRateUniqueForDateBusinessRule;
    private readonly IProjectUserLoader _projectUserLoader;
    private readonly ICanFinanceUserEditProjectRatesBusinessRule _canFinanceUserEditProjectRateBusinessRule;
    private readonly IProjectRateEncryptor _projectRateEncryptor;
    private readonly IFinanceDbContext _dbContext;

    public EditProjectRateRequest(
        IIsProjectRateUniqueForDateBusinessRule isProjectRateUniqueForDateBusinessRule,
        IProjectUserLoader projectUserLoader,
        ICanFinanceUserEditProjectRatesBusinessRule canFinanceUserEditProjectRateBusinessRule,
        IProjectRateEncryptor projectRateEncryptor,
        IFinanceDbContext dbContext)
    {
        _isProjectRateUniqueForDateBusinessRule = isProjectRateUniqueForDateBusinessRule;
        _projectUserLoader = projectUserLoader;
        _canFinanceUserEditProjectRateBusinessRule = canFinanceUserEditProjectRateBusinessRule;
        _projectRateEncryptor = projectRateEncryptor;
        _dbContext = dbContext;
    }

    public async Task<ProjectRateCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var rate = await _dbContext.ProjectRates
                .Where(x => x.Id == request.RateId)
                .Include(x => x.User)
                .Include(x => x.Project)
                .ThenInclude(x => x!.Client)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectRateNotFound);

        if (rate.IsDeleted)
        {
            throw new InvalidDataAppException(PortalMessages.ProjectRateCannotBeEdited);
        }

        var billableUserProjects = await _projectUserLoader.GetBillableUserProjectOnSpecificMonthAsync(rate.UserId, request.RateFrom,
            cancellationToken);

        if (!billableUserProjects.Contains(rate.ProjectId))
        {
            throw new InvalidDataAppException(PortalMessages.UserDoesNotHaveAccessToSomeProjects);
        }

        request.RateFrom = new DateOnly(request.RateFrom.Year, request.RateFrom.Month, 1); //first day of requested month

        if (request.RateFrom != rate.RateFrom)
        {
            var isDateInvalid = await _isProjectRateUniqueForDateBusinessRule.IsInvalid(rate.Id, rate.UserId, rate.ProjectId,
                request.RateFrom, cancellationToken);

            if (isDateInvalid)
            {
                throw new EntityConflictException(PortalMessages.ProjectRateDateConflicts);
            }
        }

        var canCreateRate = await _canFinanceUserEditProjectRateBusinessRule.IsCorrespondsToBusiness(rate.UserId, rate.ProjectId,
            request.RateFrom, cancellationToken);

        if (!canCreateRate)
        {
            throw new ActionNotAllowedException();
        }

        return await UpdateProjectRateAsync(request, rate, cancellationToken);
    }

    private async Task<ProjectRateCoreDto> UpdateProjectRateAsync(Command request, ProjectRateEntity rate, CancellationToken cancellationToken)
    {
        rate.HourlyRate = _projectRateEncryptor.Encrypt(request.HourlyRate)!;
        rate.CurrencyId = request.CurrencyId;
        rate.RateFrom = request.RateFrom;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProjectRateCoreDto
        {
            Id = rate.Id,
            UserId = rate.UserId,
            ProjectId = rate.ProjectId,
            HourlyRate = _projectRateEncryptor.Decrypt(rate.HourlyRate)!,
            CurrencyId = rate.CurrencyId,
            RateFrom = rate.RateFrom,
            User = FinanceUserCoreDto.Of(rate.User),
            Project = FinanceProjectCoreDto.Of(rate.Project),
        };
    }

    internal sealed record Command : ICommand<ProjectRateCoreDto>, ITransactionalRequest
    {
        public int RateId { get; set; }
        public decimal HourlyRate { get; set; }
        public required string CurrencyId { get; set; }
        public DateOnly RateFrom { get; set; }
    };

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.HourlyRate)
                .NotNull()
                .GreaterThanOrEqualTo(0).Money();

            RuleFor(x => x.CurrencyId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.RateFrom).NotNull();
        }
    }
}
