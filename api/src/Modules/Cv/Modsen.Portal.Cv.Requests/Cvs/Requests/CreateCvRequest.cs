using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Core.Validators;
using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests;

internal sealed class CreateCvRequest : ICommandHandler<CreateCvRequest.Command, CvCoreDto>
{
    private readonly ICvDbContext _cvDbContext;
    private readonly ICreateCvValidator _createCvValidator;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;

    public CreateCvRequest(
        ICvDbContext cvDbContext,
        ICreateCvValidator createCvValidator,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService)
    {
        _cvDbContext = cvDbContext;
        _createCvValidator = createCvValidator;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
    }

    public async Task<CvCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var requestDto = request.RequestDto;

        await _createCvValidator.ValidateAsync(CreateCvValidatorCoreDto.Of(requestDto), cancellationToken);

        var userId = _authenticatedUserContext.UserId;
        var now = _clockService.UtcNow;

        var cvToCreate = new CvEntity
        {
            CvName = requestDto.CvName,
            UserId = requestDto.UserId,
            Status = CvStatus.Draft,
            IsVisibleToUser = false,
            UserFirstName = requestDto.UserFirstName,
            UserLastName = requestDto.UserLastName,
            UserProjectPosition = requestDto.UserProjectPosition,
            UserLocation = requestDto.UserLocation,
            UserEducation = requestDto.UserEducation,
            UserSkills = requestDto.UserSkills,
            UserAbout = requestDto.UserAbout,
            Website = requestDto.Website,
            IsCorporateDesign = requestDto.IsCorporateDesign,
            IsDeleted = false,
            CreatedById = userId,
            WorkExperiences = requestDto.WorkExperiences
                .Select(x => x.ToEntity(userId, now))
                .ToList(),
            Languages = requestDto.Languages
                .Select(x => x.ToEntity(userId, now))
                .ToList(),
            ProjectExperiences = requestDto.ProjectExperiences
                .Select((x, index) => x.ToEntity(userId, now, index))
                .ToList(),
            SkillGroups = requestDto.SkillGroups
                .Select((x, index) => x.ToEntity(userId, now, index))
                .ToList()
        };

        _cvDbContext.Cvs.Add(cvToCreate);

        await _cvDbContext.SaveChangesAsync(cancellationToken);

        return CvCoreDto.Of(cvToCreate);
    }

    internal sealed record Command(CreateCvDto RequestDto) : ICommand<CvCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto)
                .SetValidator(new BaseCvRequestDtoValidator());
        }
    }
}
