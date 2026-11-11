using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;

internal sealed class DeleteSpecializationTemplateRequest
    : ICommandHandler<DeleteSpecializationTemplateRequest.Command, DeleteSpecializationTemplateRequest.Response>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ISpecializationNotAssignedToAnyUserBusinessRule _specializationNotAssignedToAnyUserBusinessRule;
    private readonly IDeleteSpecializationTemplateHardSkillsDbQuery _deleteSpecializationTemplateHardSkillsDbQuery;
    private readonly IGetSpecializationTemplateByIdDbQuery _getSpecializationTemplateByIdDbQuery;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public DeleteSpecializationTemplateRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ISpecializationNotAssignedToAnyUserBusinessRule specializationNotAssignedToAnyUserBusinessRule,
        IDeleteSpecializationTemplateHardSkillsDbQuery deleteSpecializationTemplateHardSkillsDbQuery,
        IGetSpecializationTemplateByIdDbQuery getSpecializationTemplateByIdDbQuery,
        ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _specializationNotAssignedToAnyUserBusinessRule = specializationNotAssignedToAnyUserBusinessRule;
        _deleteSpecializationTemplateHardSkillsDbQuery = deleteSpecializationTemplateHardSkillsDbQuery;
        _getSpecializationTemplateByIdDbQuery = getSpecializationTemplateByIdDbQuery;
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        await CheckIfTemplateCanBeDeleted(request.Id, cancellationToken);

        var isSpecializationAssignedToAnyUser = await _specializationNotAssignedToAnyUserBusinessRule
            .IsCorrespondsToBusiness(request.Id, cancellationToken);

        if (isSpecializationAssignedToAnyUser)
        {
            throw new ActionNotAllowedException("You can't delete specialization that is assigned to a user");
        }

        var template = await _teamBoardDbContext.SpecializationTemplates.FindAsync([request.Id], cancellationToken);

        template!.IsActive = false;
        template.UpdatedAtUtc = _clockService.UtcNow;
        template.UpdatedById = _authenticatedUserContext.UserId;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        await _deleteSpecializationTemplateHardSkillsDbQuery.ExecuteAsync(
            request.Id,
            hardSkillsIds: null,
            cancellationToken: cancellationToken);

        var response = new Response(template.Id, template.Name);

        return response;
    }

    private async Task CheckIfTemplateCanBeDeleted(int id, CancellationToken cancellationToken)
    {
        var template = await _getSpecializationTemplateByIdDbQuery.ExecuteAsync(id, cancellationToken) ??
            throw new EntityNotFoundException($"Specialization by id {id} is not found");

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() ||
            _authenticatedUserContext.GetRoles().IsGroupManager())
        {
            var userDepartmentId = _authenticatedUserContext.DepartmentId;

            var canHeadOrGmDeleteSpecialization =
                template.CreatedById == _authenticatedUserContext.GetRequiredUserId() ||
                template.SpecializationTemplateDepartments.Any(department => department.DepartmentId == userDepartmentId);

            if (!canHeadOrGmDeleteSpecialization)
            {
                throw new ActionNotAllowedException("Specialization hasn't been created by current user or your department" +
                    " is not related to specialization departments, you don't have permissions to perform this action");
            }
        }
    }

    internal sealed record Command(int Id) : ICommand<Response>, ITransactionalRequest;

    internal record Response(int Id, string Name);

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEqual(0)
                .WithMessage("This field is required");
        }
    }
}
