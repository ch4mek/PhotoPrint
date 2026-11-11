using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Education.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Education.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Education.Requests;

internal sealed class UpdateUserEducationRequest : ICommandHandler<UpdateUserEducationRequest.Command, UserEducationDto>
{
    private readonly ICanUpdateUserEducationBusinessRule _canUpdateEducationBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    private readonly IEducationLevelOrderWeightCalculator _educationLevelOrderWeightCalculator;
    private readonly IStudyFormOrderWeightCalculator _studyFormOrderWeightCalculator;

    public UpdateUserEducationRequest(ITeamBoardDbContext teamBoardDbContext,
        ICanUpdateUserEducationBusinessRule canUpdateEducationBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        IEducationLevelOrderWeightCalculator educationLevelOrderWeightCalculator,
        IStudyFormOrderWeightCalculator studyFormOrderWeightCalculator)
    {
        _canUpdateEducationBusinessRule = canUpdateEducationBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
        _educationLevelOrderWeightCalculator = educationLevelOrderWeightCalculator;
        _studyFormOrderWeightCalculator = studyFormOrderWeightCalculator;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<UserEducationDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var updateDto = request.UpdateDto;

        var education = await _teamBoardDbContext.UserEducations
            .FirstOrDefaultAsync(edu => edu.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.EducationNotFound);

        if (_authenticatedUserContext.GetRequiredUserId() == education.UserId)
        {
            throw new ActionNotAllowedException("You are not allowed to update your own education");
        }

        var isCorrespondsToBusiness = await _canUpdateEducationBusinessRule.IsCorrespondsToRuleAsync(education.UserId,
            cancellationToken);

        if (!isCorrespondsToBusiness)
        {
            throw new ActionNotAllowedException("You are not allowed to update a user education");
        }

        education.EducationLevel = updateDto.EducationLevel;
        education.EducationalInstitution = updateDto.EducationalInstitution;
        education.Faculty = updateDto.Faculty;
        education.Specialization = updateDto.Specialization;
        education.StudyForm = updateDto.StudyForm;
        education.GraduationYear = updateDto.GraduationYear;
        education.Comment = updateDto.Comment;
        education.UpdatedAtUtc = DateTime.UtcNow;
        education.UpdatedById = _authenticatedUserContext.UserId;

        education.EducationLevelOrderWeight = _educationLevelOrderWeightCalculator
            .GetOrderWeight(updateDto.EducationLevel);
        education.StudyFormOrderWeight = _studyFormOrderWeightCalculator.GetOrderWeight(updateDto.StudyForm);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return UserEducationDto.Of(education);
    }

    internal sealed record Command(UpdateUserEducationDto UpdateDto, int Id) : ICommand<UserEducationDto>,
        ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull();

            RuleFor(x => x.UpdateDto).SetValidator(new UpdateUserEducationDto.Validator());
        }
    }

}
