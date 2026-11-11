using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.Loaders;

internal interface IUserPersonalInfoLoader
{
    Task<PersonalInfoCoreDto> LoadAsync(int userId, CancellationToken cancellationToken);
}

internal class UserPersonalInfoLoader : IUserPersonalInfoLoader
{
    private readonly TeamBoardDbContext _teamBoardDbContext;

    public UserPersonalInfoLoader(TeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<PersonalInfoCoreDto> LoadAsync(int userId, CancellationToken cancellationToken)
    {
        var userPosition = (await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == userId)
            .Select(user => user.Position)
            .FirstOrDefaultAsync(cancellationToken));

        var isIntern = userPosition.IsIntern();

        var currentDate = DateTime.UtcNow.Date;

        var baseQuery = _teamBoardDbContext.TeamBoardUsers
            .AsNoTracking()
            .Where(user => user.UserId == userId)
            .Include(user => user.EnglishLevel)
            .Include(user => user.Department)
            .Include(user => user.RegisteredLocations)
                .ThenInclude(locationHistory => locationHistory.RegisteredLocation)
            .AsSplitQuery();          

        if (!isIntern)
        {
            baseQuery = baseQuery
                .Include(user => user.UserPayloadDetails)
                .Include(user => user.UserPerformanceReviews)
                .Include(user => user.UserSpecializations)
                    .ThenInclude(specialization => specialization.SeniorityLevel)
                .Include(user => user.UserSpecializations)
                    .ThenInclude(specialization => specialization.SpecializationTemplate)
                .Include(user => user.UserSoftSkillsSnapshots)
                    .ThenInclude(softSkillsSnapshot => softSkillsSnapshot.SoftSkills)
                    .ThenInclude(softSkill => softSkill.SeniorityLevel);
        }

        var selectQuery = baseQuery
            .Select(user => new PersonalInfoProjection
            (            
                $"{user.LastName} {user.FirstName}",
                $"{user.LastNameRu} {user.FirstNameRu} {user.PatronymicRu}",
                user.FirstName,
                user.LastName,
                user.FirstNameRu,
                user.LastNameRu,
                user.PatronymicRu,
                user.Email,
                user.PersonalEmail,
                user.PersonalEmailSecond,
                user.PersonalPhone,
                user.Telegram,
                user.LinkedIn,
                user.Position,
                user.DateOfBirth,
                isIntern ? null :user.UserPayloadDetails.FirstOrDefault(),
                user.Location,
                isIntern
                    ? null
                    : user.UserSpecializations
                        .FirstOrDefault(specialization =>
                            specialization.IsPrimary &&
                            specialization.UserId == userId),
                user.DepartmentId,
                user.Department!.FullName,
                user.ResourceGroupId,
                user.EnglishLevelId,
                user.Comments,
                user.AvatarColor,
                user.AvatarFileId,
                isIntern ? null : user.UserSoftSkillsSnapshots,
                isIntern
                    ? null
                    : user.UserPerformanceReviews
                        .Where(perfReview => perfReview.UserId == userId)
                        .Where(perfReview => perfReview.InterviewDateFromUtc >= currentDate)
                        .Where(perfReview => perfReview.Status != UserPerformanceReviewStatus.Passed)
                        .OrderBy(perfReview => perfReview.InterviewDateFromUtc)
                        .Select(perfReview => new PerformanceReviewProjection(
                            perfReview.InterviewDateFromUtc,
                            perfReview.Status))
                        .FirstOrDefault(),
                isIntern
                    ? null
                    : user.UserPerformanceReviews
                        .Where(perfReview => perfReview.UserId == userId)
                        .Where(perfReview => perfReview.InterviewDateFromUtc <= currentDate)
                        .OrderByDescending(perfReview => perfReview.InterviewDateFromUtc)
                        .Select(perfReview => new PerformanceReviewProjection(
                            perfReview.InterviewDateFromUtc,
                            perfReview.Status)),
                user.RegisteredLocations
                        .FirstOrDefault(locationHistory => locationHistory.DateTo == null)!.RegisteredLocation!.Name,
                user.RegisteredLocations
                        .FirstOrDefault(locationHistory => locationHistory.DateTo == null)!.RegisteredLocation!.Id,
                user.CurrentLocation,
                user.MilitaryServiceStatus,
                user.IsGraduateJobPlacement
            ));

        var result = await selectQuery.FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        var resourceGroup = await _teamBoardDbContext.TeamBoardResourceGroups
            .Where(x => x.ResourceGroupId == result.ResourceGroupId && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
        var resourceGroupName = resourceGroup?.ResourceGroupName;

        var response = FormBasePersonalInfoCoreDto(result, resourceGroupName);

        if (isIntern)
        {
            return response;
        }

        var activeSoftSkills = await _teamBoardDbContext.SoftSkills.ToListAsync(cancellationToken);

        var futureCurrentPerformanceReview = result.FuturePerformanceReview;
        var pastCurrentPerformanceReview = result.PastPerformanceReviews!
            .TakeWhile(performanceReview =>
                performanceReview.PerformanceReviewDateStatus != UserPerformanceReviewStatus.Passed)
            .FirstOrDefault();

        if (futureCurrentPerformanceReview is not null && pastCurrentPerformanceReview is not null)
        {
            var diffFuture = futureCurrentPerformanceReview.UpcomingPerfomanceReview - currentDate;
            var diffPast = currentDate - pastCurrentPerformanceReview.UpcomingPerfomanceReview;

            if (diffFuture > diffPast)
            {
                futureCurrentPerformanceReview = pastCurrentPerformanceReview;
            }
        }
        else if (futureCurrentPerformanceReview is null && pastCurrentPerformanceReview is not null)
        {
            futureCurrentPerformanceReview = pastCurrentPerformanceReview;
        }

        response = response with
        {
            SeniorityLevel = result.UserPrimarySpecialization?.SeniorityLevel?.InternalName,
            SeniorityLevelCoefficient = result.UserPrimarySpecialization?.SeniorityLevel?.Coefficient,
            PrimarySpecialization = result.UserPrimarySpecialization?.SpecializationTemplate?.Name,
            NeedProject = result.PayloadDetails?.NeedProject ?? false,
            UserSoftSkillsSnapshots = result.UserSoftSkillsSnapshots!
                .Select(skillsSnapshot => new UserSoftSkillsSnapshotCoreDto(
                    skillsSnapshot.SoftSkills.Select(softSkill =>
                        new UserSoftSkillCoreDto(
                            softSkill.SoftSkillId,
                            softSkill.SeniorityLevel!.ExternalName,
                            softSkill.SeniorityLevel.OrderWeight,
                            softSkill.SeniorityLevel.Coefficient,
                            activeSoftSkills.First(skill => skill.Id == softSkill.SoftSkillId).IsActive)).ToList(),
                    skillsSnapshot.SnapshotDate,
                    skillsSnapshot.Id)).ToList(),
            UpcomingPerformanceReview = futureCurrentPerformanceReview?.UpcomingPerfomanceReview,
            PerformanceReviewDateStatus = futureCurrentPerformanceReview?.PerformanceReviewDateStatus.GetDescription()
        };              

        return response;
    }

    private static PersonalInfoCoreDto FormBasePersonalInfoCoreDto(PersonalInfoProjection result, string? resourceGroupName)
    {
        return new PersonalInfoCoreDto(
            result.FullName,
            result.FullNameRu,
            result.FirstName,
            result.LastName,
            result.FirstNameRu,
            result.LastNameRu,
            result.PatronymicRu,
            result.Email,
            result.PersonalEmail,
            result.PersonalEmailSecond,
            result.PersonalPhone,
            result.Telegram,
            result.LinkedIn,
            result.Position,
            result.DateOfBirth,
            result.Location,
            null,
            null,
            null,
            result.DepartmentId,
            result.DepartmentFullName,
            result.ResourceGroupId,
            resourceGroupName,
            result.EnglishLevelId,
            result.Comments,
            result.AvatarColor,
            result.AvatarFileId,
            null,
            null,
            null,
            null,
            result.RegisteredLocation,
            result.RegisteredLocationId,
            result.CurrentLocation,
            result.MilitaryServiceStatus,
            result.IsGraduateJobPlacement);
    }

    internal sealed record PersonalInfoProjection(
        string FullName,
        string FullNameRu,
        string FirstName,
        string LastName,
        string? FirstNameRu,
        string? LastNameRu,
        string? PatronymicRu,
        string Email,
        string? PersonalEmail,
        string? PersonalEmailSecond,
        string? PersonalPhone,
        string? Telegram,
        string? LinkedIn,
        string? Position,
        DateOnly? DateOfBirth,
        UserPayloadDetails? PayloadDetails,
        string? Location,
        UserSpecialization? UserPrimarySpecialization,
        string? DepartmentId,
        string DepartmentFullName,
        int? ResourceGroupId,
        string? EnglishLevelId,
        string? Comments,
        string? AvatarColor,
        string? AvatarFileId,
        List<UserSoftSkillsSnapshot>? UserSoftSkillsSnapshots,
        PerformanceReviewProjection? FuturePerformanceReview,
        IEnumerable<PerformanceReviewProjection>? PastPerformanceReviews,
        string RegisteredLocation,
        int RegisteredLocationId,
        string? CurrentLocation,
        MilitaryServiceStatus? MilitaryServiceStatus,
        bool? IsGraduateJobPlacement);

    internal sealed record PerformanceReviewProjection(
        DateTime? UpcomingPerfomanceReview,
        UserPerformanceReviewStatus PerformanceReviewDateStatus);
}
