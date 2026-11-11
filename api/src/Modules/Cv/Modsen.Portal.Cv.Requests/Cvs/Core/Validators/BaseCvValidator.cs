using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.Cvs.Core.BusinessRules;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.Validators;

internal abstract class BaseCvValidator
{
    private readonly ICvNameUniqueWithinUserBusinessRule _cvNameUniqueWithinUserBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICvDbContext _cvDbContext;

    protected BaseCvValidator(
        ICvNameUniqueWithinUserBusinessRule cvNameUniqueWithinUserBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        ICvDbContext cvDbContext)
    {
        _cvNameUniqueWithinUserBusinessRule = cvNameUniqueWithinUserBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
        _cvDbContext = cvDbContext;
    }

    protected async Task BaseValidate(BaseCvValidatorCoreDto coreDto, int? cvId, CancellationToken cancellationToken)
    {
        await IsNameUniqueWithinUser(coreDto, cvId, cancellationToken);

        IsWorkExperiencesUniqueWithinCv(coreDto);
        IsLanguagesUniqueWithinCv(coreDto);
        IsSkillGroupNameUniqueWithinCv(coreDto);
        IsSkillGroupNameRuUniqueWithinCv(coreDto);
        IsSkillsUniqueWithinCv(coreDto);

        await IsUserExists(coreDto, cancellationToken);
    }

    private async Task IsNameUniqueWithinUser(BaseCvValidatorCoreDto coreDto, int? cvId, CancellationToken cancellationToken)
    {
        var isNameUniqueWithinUser = await _cvNameUniqueWithinUserBusinessRule
            .IsCorrespondsToBusiness(coreDto.Name, coreDto.UserId, cvId, cancellationToken);
        if (!isNameUniqueWithinUser)
        {
            throw new InvalidDataAppException(PortalMessages.CvNameExists);
        }
    }

    private static void IsWorkExperiencesUniqueWithinCv(BaseCvValidatorCoreDto coreDto)
    {
        var isWorkExperiencesUniqueWithinCv =
            coreDto.WorkExperiences.DistinctBy(x => x.Specialization).Count() ==
            coreDto.WorkExperiences.Count;
        if (!isWorkExperiencesUniqueWithinCv)
        {
            throw new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSpecialization);
        }
    }

    private static void IsLanguagesUniqueWithinCv(BaseCvValidatorCoreDto coreDto)
    {
        var isLanguagesUniqueWithinCv =
            coreDto.Languages.DistinctBy(x => x.Name).Count() ==
            coreDto.Languages.Count;
        if (!isLanguagesUniqueWithinCv)
        {
            throw new InvalidDataAppException(PortalMessages.EnteredIdenticalCvLanguages);
        }
    }

    private static void IsSkillGroupNameUniqueWithinCv(BaseCvValidatorCoreDto coreDto)
    {
        var isSkillGroupNameUniqueWithinCv =
            coreDto.SkillGroups.DistinctBy(x => x.Name).Count() ==
            coreDto.SkillGroups.Count;
        if (!isSkillGroupNameUniqueWithinCv)
        {
            throw new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSkillGroupNames);
        }
    }

    private static void IsSkillGroupNameRuUniqueWithinCv(BaseCvValidatorCoreDto coreDto)
    {
        var isSkillGroupNameRuUniqueWithinCv =
            coreDto.SkillGroups.DistinctBy(x => x.NameRu).Count() ==
            coreDto.SkillGroups.Count;
        if (!isSkillGroupNameRuUniqueWithinCv)
        {
            throw new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSkillGroupRuNames);
        }
    }

    private static void IsSkillsUniqueWithinCv(BaseCvValidatorCoreDto coreDto)
    {
        var allCvSkills = coreDto.SkillGroups.SelectMany(x => x.Skills).ToList();
        var isSkillsUniqueWithinCv =
            allCvSkills.DistinctBy(x => x.Name).Count() ==
            allCvSkills.Count;

        if (!isSkillsUniqueWithinCv)
        {
            throw new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSkills);
        }
    }

    private async Task IsUserExists(BaseCvValidatorCoreDto coreDto, CancellationToken cancellationToken)
    {
        var query = _cvDbContext.CvUsers.AsNoTracking();

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(x => x.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query.Where(x => x.UserId == _authenticatedUserContext.UserId);
        }

        _ = await query
                .Where(x => x.IsActive && x.UserId == coreDto.UserId)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);
    }
}
