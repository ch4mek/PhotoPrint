using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Core.DbQueries;
using Modsen.Portal.Cv.Requests.Cvs.Core.Validators;
using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests;

internal sealed class UpdateCvRequest : ICommandHandler<UpdateCvRequest.Command, CvCoreDto>
{
    private readonly IGetCvByIdWithIncludesDbQuery _getCvByIdWithIncludesDbQuery;
    private readonly IUpdateCvValidator _updateCvValidator;
    private readonly ICvDbContext _cvDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;

    public UpdateCvRequest(
        IGetCvByIdWithIncludesDbQuery getCvByIdWithIncludesDbQuery,
        IUpdateCvValidator updateCvValidator,
        ICvDbContext cvDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService)
    {
        _getCvByIdWithIncludesDbQuery = getCvByIdWithIncludesDbQuery;
        _updateCvValidator = updateCvValidator;
        _cvDbContext = cvDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
    }

    public async Task<CvCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var requestDto = request.RequestDto;

        var cv = await _getCvByIdWithIncludesDbQuery.ExecuteAsync(requestDto.Id, cancellationToken);

        if (cv is null)
        {
            throw new EntityNotFoundException(PortalMessages.CvNotFound);
        }

        await _updateCvValidator.ValidateAsync(UpdateCvValidatorCoreDto.Of(requestDto), cancellationToken);

        var userId = _authenticatedUserContext.UserId;
        var now = _clockService.UtcNow;

        cv.CvName = requestDto.CvName;
        cv.UserId = requestDto.UserId;
        cv.UserFirstName = requestDto.UserFirstName;
        cv.UserLastName = requestDto.UserLastName;
        cv.UserProjectPosition = requestDto.UserProjectPosition;
        cv.UserLocation = requestDto.UserLocation;
        cv.UserEducation = requestDto.UserEducation;
        cv.UserSkills = requestDto.UserSkills;
        cv.UserAbout = requestDto.UserAbout;
        cv.Website = requestDto.Website;
        cv.IsCorporateDesign = requestDto.IsCorporateDesign;

        _cvDbContext.WorkExperiences.RemoveRange(cv.WorkExperiences);
        _cvDbContext.Languages.RemoveRange(cv.Languages);
        _cvDbContext.ProjectExperiences.RemoveRange(cv.ProjectExperiences);
        _cvDbContext.Skills.RemoveRange(cv.SkillGroups.SelectMany(x => x.Skills).ToList());
        _cvDbContext.SkillGroups.RemoveRange(cv.SkillGroups);

        cv.WorkExperiences.Clear();
        cv.WorkExperiences = requestDto.WorkExperiences
            .Select(x => x.ToEntity(userId, now))
            .ToList();

        cv.Languages.Clear();
        cv.Languages = requestDto.Languages
            .Select(x => x.ToEntity(userId, now))
            .ToList();

        cv.ProjectExperiences.Clear();
        cv.ProjectExperiences = requestDto.ProjectExperiences
            .Select((x, index) => x.ToEntity(userId, now, index))
            .ToList();

        cv.SkillGroups.Clear();
        cv.SkillGroups = requestDto.SkillGroups
            .Select((x, index) => x.ToEntity(userId, now, index))
            .ToList();

        cv.UpdatedAtUtc = _clockService.UtcNow;
        cv.UpdatedById = _authenticatedUserContext.GetRequiredUserId();

        await _cvDbContext.SaveChangesAsync(cancellationToken);

        return CvCoreDto.Of(cv);
    }

    internal sealed record Command(UpdateCvDto RequestDto) : ICommand<CvCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto)
                .SetValidator(new BaseCvRequestDtoValidator());
        }
    }
}
