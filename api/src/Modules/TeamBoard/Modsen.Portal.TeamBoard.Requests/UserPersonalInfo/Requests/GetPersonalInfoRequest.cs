using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Resolvers;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;

internal sealed class GetPersonalInfoRequest : IQueryHandler<GetPersonalInfoRequest.Query, PersonalInfoDto>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canUserBeRetrievedBusinessRule;
    private readonly ISpecializationSeniorityResolver _specializationSeniorityResolver;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;
    private readonly IPerformanceReviewDateTypeCalculator _reviewDateTypeCalculator;
    private readonly IUserPersonalInfoLoader _personalInfoLoader;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetPersonalInfoRequest(ICanTeamBoardUserBeRetrievedBusinessRule canUserBeRetrievedBusinessRule,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor,
        ISpecializationSeniorityResolver specializationSeniorityResolver,
        IUserPersonalInfoLoader personalInfoLoader,
        ITeamBoardDbContext teamBoardDbContext,
        IPerformanceReviewDateTypeCalculator reviewDateTypeCalculator,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canUserBeRetrievedBusinessRule = canUserBeRetrievedBusinessRule;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _specializationSeniorityResolver = specializationSeniorityResolver;
        _personalInfoLoader = personalInfoLoader;
        _teamBoardDbContext = teamBoardDbContext;
        _reviewDateTypeCalculator = reviewDateTypeCalculator;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PersonalInfoDto> Handle(Query request, CancellationToken cancellationToken)
    {
        await EnsureAccess(request, cancellationToken);

        var userInfo = await _personalInfoLoader.LoadAsync(request.UserId, cancellationToken);

        return await BuildResult(request, userInfo, cancellationToken);
    }

    private async Task EnsureAccess(Query request, CancellationToken token)
    {
        var accessOptions =
            AccessOptions.Self |
            AccessOptions.Manager |
            AccessOptions.Hr |
            AccessOptions.Accountant |
            AccessOptions.Coordinator;

        var canBeRetrieved = await _canUserBeRetrievedBusinessRule
            .IsCorrespondsToBusiness(request.UserId, accessOptions, token);

        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException(
                $"You can't have access to user with id {request.UserId}");
        }
    }

    private async Task<PersonalInfoDto> BuildResult(
        Query request,
        PersonalInfoCoreDto userInfo,
        CancellationToken token)
    {
        if (CanSeeFullInfo(request.UserId, userInfo))
        {
            return await BuildFullInfo(request, userInfo, token);
        }

        return BuildRestrictedInfo(userInfo);
    }

    private async Task<PersonalInfoDto> BuildFullInfo(
        Query request,
        PersonalInfoCoreDto userInfo,
        CancellationToken token)
    {
        PersonalInfoDto result;

        if (userInfo.Position.IsIntern())
        {
            result = PersonalInfoDto.Of(userInfo, null, null);
        }
        else
        {
            var generalSeniority = CalculateGeneralSeniority(userInfo);

            var softSkillSeniority = await GetSignedSoftSkillSeniorityLevel(
                userInfo.UserSoftSkillsSnapshots!,
                token);

            var finalGeneralSeniority = await ResolveSeniorityLevel(generalSeniority, token);

            result = PersonalInfoDto.Of(userInfo, softSkillSeniority, finalGeneralSeniority);

            await ApplyPerformanceReviewInfo(result, request, userInfo, token);
        }

        if (IsCurrentUser(request.UserId))
        {
            HideSensitiveFields(result);
        }

        return result;
    }

    private async Task<string?> ResolveSeniorityLevel(double? generalSeniority, CancellationToken token)
    {
        if (generalSeniority == null)
        {
            return null;
        }

        return await GetSeniorityLevelByCoefficientAsync(generalSeniority.Value, token);
    }

    private async Task ApplyPerformanceReviewInfo(
        PersonalInfoDto result,
        Query request,
        PersonalInfoCoreDto userInfo,
        CancellationToken token)
    {
        if (userInfo.UpcomingPerformanceReview is null)
        {
            result.UpcomingPerformanceReview = null;
            result.PerformanceReviewDateStatus = null;
            return;
        }

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(token);
        var overdueDays = request.OverdueDaysCount ?? settings.OverdueDaysCount;

        result.UpcomingPerformanceReview = userInfo.UpcomingPerformanceReview;

        result.PerformanceReviewDateStatus =
            userInfo.PerformanceReviewDateStatus == "Passed"
                ? PerformanceReviewDateType.Passed.GetDescription()
                : _reviewDateTypeCalculator.Calculate(
                    userInfo.UpcomingPerformanceReview.Value.Date,
                    overdueDays,
                    settings.UpcomingIminentBorderDaysCount);
    }

    private static void HideSensitiveFields(PersonalInfoDto result)
    {
        result.NeedProject = null;
        result.Comments = null;
    }

    private static PersonalInfoDto BuildRestrictedInfo(PersonalInfoCoreDto userInfo)
    {
        var result = PersonalInfoDto.Of(userInfo, null, null);

        result.Seniority = null;
        result.PrimarySpecialization = null;
        result.NeedProject = null;
        result.Comments = null;
        result.GeneralSeniority = null;

        return result;
    }

    private static double? CalculateGeneralSeniority(PersonalInfoCoreDto userInfo)
    {
        var avg = CalculateAverageSoftSkillSeniority(userInfo.UserSoftSkillsSnapshots);
        var value = (avg + userInfo.SeniorityLevelCoefficient) / 2;

        return value != null ? CustomRound(value.Value) : null;
    }

    private bool CanSeeFullInfo(int userId, PersonalInfoCoreDto userInfo)
    {
        return IsUserManager(userInfo) || IsCurrentUser(userId);
    }

    private bool IsCurrentUser(int userId)
    {
        return _authenticatedUserContext.UserId == userId;
    }

    private bool IsUserManager(PersonalInfoCoreDto user)
    {
        var userRoles = _authenticatedUserContext.GetRoles();

        if (user.DepartmentId == _authenticatedUserContext.DepartmentId &&
            user.DepartmentId != null &&
            userRoles.IsHeadOfDepartment())
        {
            return true;
        }

        if (user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId &&
            user.ResourceGroupId != null &&
            userRoles.IsGroupManager())
        {
            return true;
        }

        if (userRoles.IsCto() || userRoles.IsCeo())
        {
            return true;
        }

        return false;
    }

    private Task<string?> GetSeniorityLevelByCoefficientAsync(double coefficient, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.SeniorityLevels
            .Where(seniorityLevel => seniorityLevel.Coefficient == (int)coefficient)
            .Select(seniorityLevel => seniorityLevel.ExternalName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<string?> GetSignedSoftSkillSeniorityLevel(IReadOnlyCollection<UserSoftSkillsSnapshotCoreDto> snapshots,
        CancellationToken cancellationToken)
    {
        var latestSnapshot = snapshots
            .Where(snapshot => snapshot.UserSoftSkills.Any(skill => skill.IsActive))
            .OrderByDescending(x => x.SnapshotDate)
            .ThenByDescending(x => x.SnapShotId).FirstOrDefault();
        if (latestSnapshot == null)
        {
            return null;
        }

        var skills = latestSnapshot.UserSoftSkills.Where(skillCoreDto => skillCoreDto.IsActive).ToList();

        if (skills.Count == 0)
        {
            return null;
        }

        var softSkillSignedSeniorityLevel = await _specializationSeniorityResolver
                .ResolveAsync(skills.Select(x => new HardSkillSeniorityDto
                {
                    Id = x.SoftSkillId,
                    SeniorityLevelId = x.SeniorityLevelId
                }).ToArray(), cancellationToken);

        var signedSeniority = await GetSeniorityLevelAsync(softSkillSignedSeniorityLevel, cancellationToken)
            ?? throw new EntityNotFoundException();
        return signedSeniority;
    }

    private static double? CalculateAverageSoftSkillSeniority(IReadOnlyCollection<UserSoftSkillsSnapshotCoreDto> snapshots)
    {
        var latestSnapshot = snapshots.MaxBy(x => x.SnapshotDate);
        if (latestSnapshot == null)
        {
            return null;
        }

        var softSkills = latestSnapshot.UserSoftSkills;

        if (softSkills.Count != 0)
        {
            var numinator = softSkills.Sum(softSkillCoreDto => softSkillCoreDto.SoftSkillSeniorityCoefficient);
            var denominator = softSkills.Count;
            var obtainedValue = (double)(numinator / denominator)!;
            var roundedValue = CustomRound(obtainedValue);

            return roundedValue;
        }

        return null;
    }

    private static double? CustomRound(double value)
    {
        var tenths = (int)(value * 10) % 10;

        if (tenths is >= 6 and <= 9)
        {
            return Math.Ceiling(value);
        }

        return Math.Floor(value);
    }

    private Task<string?> GetSeniorityLevelAsync(string id, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.SeniorityLevels
                .Where(seniorityLevel => seniorityLevel.Id == id)
                .Select(seniorityLevel => seniorityLevel.InternalName)
                .FirstOrDefaultAsync(cancellationToken);
    }

    internal sealed record Query(int UserId, int? OverdueDaysCount) : IQuery<PersonalInfoDto>;
}
