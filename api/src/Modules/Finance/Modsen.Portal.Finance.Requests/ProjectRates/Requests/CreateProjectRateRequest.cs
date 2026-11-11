using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Composers;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Loaders;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Requests;

internal sealed class CreateProjectRateRequest : ICommandHandler<CreateProjectRateRequest.Command, ProjectRateCoreDto>
{
    private readonly IIsProjectRateUniqueForDateBusinessRule _isProjectRateUniqueForDateBusinessRule;
    private readonly IProjectUserLoader _projectUserLoader;
    private readonly ICanFinanceUserEditProjectRatesBusinessRule _canFinanceUserEditProjectRateBusinessRule;
    private readonly IProjectRateComposer _projectRateComposer;
    private readonly IFinanceDbContext _dbContext;

    public CreateProjectRateRequest(
        IIsProjectRateUniqueForDateBusinessRule isProjectRateUniqueForDateBusinessRule,
        IProjectUserLoader projectUserLoader,
        ICanFinanceUserEditProjectRatesBusinessRule canFinanceUserEditProjectRateBusinessRule,
        IProjectRateComposer projectRateComposer,
        IFinanceDbContext dbContext)
    {
        _isProjectRateUniqueForDateBusinessRule = isProjectRateUniqueForDateBusinessRule;
        _projectUserLoader = projectUserLoader;
        _canFinanceUserEditProjectRateBusinessRule = canFinanceUserEditProjectRateBusinessRule;
        _projectRateComposer = projectRateComposer;
        _dbContext = dbContext;
    }

    public async Task<ProjectRateCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!await _dbContext.FinanceUsers
                .Where(x => x.Id == request.UserId)
                .Where(x => x.IsActive)
                .AnyAsync(cancellationToken))
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        var project = await _dbContext.FinanceProjects
                .Where(x => x.Id == request.ProjectId)
                .Where(x => x.IsActive)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

        if (project.IsPublic)
        {
            throw new ActionNotAllowedException(PortalMessages.ProjectRatesOnlyForPrivateProjects);
        }

        var billableUserProjects = await _projectUserLoader.GetBillableUserProjectOnSpecificMonthAsync(request.UserId, request.RateFrom,
            cancellationToken);

        if (!billableUserProjects.Contains(request.ProjectId))
        {
            throw new InvalidDataAppException(PortalMessages.UserDoesNotHaveAccessToSomeProjects);
        }

        request.RateFrom = new DateOnly(request.RateFrom.Year, request.RateFrom.Month, 1); //first day of requested month

        var isDateInvalid = await _isProjectRateUniqueForDateBusinessRule.IsInvalid(null, request.UserId, request.ProjectId,
            request.RateFrom, cancellationToken);

        if (isDateInvalid)
        {
            throw new EntityConflictException(PortalMessages.ProjectRateDateConflicts);
        }

        var canCreateRate = await _canFinanceUserEditProjectRateBusinessRule.IsCorrespondsToBusiness(request.UserId,
            request.ProjectId, request.RateFrom, cancellationToken);

        if (!canCreateRate)
        {
            throw new ActionNotAllowedException();
        }

        var createRateCoreDto = new CreateProjectRateCoreDto(request.UserId, request.ProjectId,
            request.HourlyRate, request.CurrencyId, request.RateFrom);

        return await _projectRateComposer.ComposeAsync(createRateCoreDto, cancellationToken);
    }

    internal sealed record Command : ICommand<ProjectRateCoreDto>, ITransactionalRequest
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public decimal HourlyRate { get; set; }
        public required string CurrencyId { get; set; }
        public DateOnly RateFrom { get; set; }
    };

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull();

            RuleFor(x => x.ProjectId)
                .NotNull();

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
