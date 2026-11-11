using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Education.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Education.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Education.Requests;

internal sealed class CreateUserEducationRequest : ICommandHandler<CreateUserEducationRequest.Command, UserEducationDto>
{
    private readonly ICanUpdateUserEducationBusinessRule _canCreateEducationBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    private readonly IEducationLevelOrderWeightCalculator _educationLevelOrderWeightCalculator;
    private readonly IStudyFormOrderWeightCalculator _studyFormOrderWeightCalculator;

    public CreateUserEducationRequest(ICanUpdateUserEducationBusinessRule canCreateEducationBusinessRule,
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IEducationLevelOrderWeightCalculator educationLevelOrderWeightCalculator,
        IStudyFormOrderWeightCalculator studyFormOrderWeightCalculator)
    {
        _canCreateEducationBusinessRule = canCreateEducationBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _educationLevelOrderWeightCalculator = educationLevelOrderWeightCalculator;
        _studyFormOrderWeightCalculator = studyFormOrderWeightCalculator;
    }

    public async Task<UserEducationDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var createDto = request.CreateDto;

        if (_authenticatedUserContext.GetRequiredUserId() == request.CreateDto.UserId)
        {
            throw new ActionNotAllowedException("You are not allowed to create your own education");
        }

        var isCorrespondsToBusiness = await _canCreateEducationBusinessRule.IsCorrespondsToRuleAsync(createDto.UserId,
            cancellationToken);

        if (!isCorrespondsToBusiness)
        {
            throw new ActionNotAllowedException("You are not allowed to create a user education");
        }

        var educationToCreate = new UserEducationEntity
        {
            UserId = createDto.UserId,
            EducationLevel = createDto.EducationLevel,
            EducationalInstitution = createDto.EducationalInstitution,
            Faculty = createDto.Faculty,
            Specialization = createDto.Specialization,
            StudyForm = createDto.StudyForm,
            GraduationYear = createDto.GraduationYear,
            Comment = createDto.Comment,
            CreatedById = _authenticatedUserContext.UserId,
            CreatedAtUtc = DateTime.UtcNow,
            EducationLevelOrderWeight = _educationLevelOrderWeightCalculator.GetOrderWeight(createDto.EducationLevel),
            StudyFormOrderWeight = _studyFormOrderWeightCalculator.GetOrderWeight(createDto.StudyForm)
        };

        _teamBoardDbContext.UserEducations.Add(educationToCreate);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return UserEducationDto.Of(educationToCreate);
    }

    internal sealed record Command(CreateUserEducationDto CreateDto) : ICommand<UserEducationDto>,
        ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CreateDto).SetValidator(new CreateUserEducationDto.Validator());
        }
    }
}
