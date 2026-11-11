using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpTeamBoardUser
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public DateOnly? CareerStartedAt { get; set; }

    public string? EnglishLevelId { get; set; }
    public string? EnglishLevelName { get; set; }
    public int? EnglishLevelOrderWeight { get; set; }

    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevel { get; set; }
    public string? GeneralSeniorityLevel { get; set; }
    public int? SeniorityLevelOrderWeight { get; set; }

    public int? PrimarySpecializationId { get; set; }
    public string? PrimarySpecializationName { get; set; }

    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }

    public string? DepartmentId { get; set; }
    public string? DepartmentFullName { get; set; }
    public int? DepartmentOrderWeight { get; set; }

    public string? Position { get; set; }
    public int? PositionOrderWeight { get; set; }
    public string? Location { get; set; }
    public string? RegisteredLocation { get; set; }
    public string? CurrentLocation { get; set; }
    public string? Comments { get; set; }

    public bool IsActive { get; set; }

    public IReadOnlyCollection<OdpNonPrimarySpecialization> OdpNonPrimarySpecializations { get; set; } = [];
    public IReadOnlyCollection<OdpUserPerformanceReview> OdpUserPerformanceReviews { get; set; } = [];

    internal static readonly Expression<Func<TeamBoardUser, OdpTeamBoardUser>> MapTeamBoardUser =
        x => new OdpTeamBoardUser
        {
            Id = x.UserId,
            FirstName = x.FirstName,
            LastName = x.LastName,

            FullName = x.LastNameRu == null && x.FirstNameRu == null
                ? x.LastName + " " + x.FirstName
                : x.LastNameRu != null && x.FirstNameRu != null
                    ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                    : (x.LastNameRu ?? x.FirstNameRu) + " (" + x.LastName + " " + x.FirstName + ")",

            CareerStartedAt = x.CareerStartedAt,

            PrimarySpecializationId = x.UserSpecializations
                .Where(sp => sp.IsPrimary)
                .Select(sp => (int?)sp.Id)
                .FirstOrDefault(),

            PrimarySpecializationName = x.UserSpecializations
                .Where(sp => sp.IsPrimary)
                .Select(sp => sp.SpecializationTemplate!.Name)
                .First(),

            ResourceGroupId = x.ResourceGroupId,
            ResourceGroupName = x.ResourceGroup != null ? x.ResourceGroup.ResourceGroupName : null,
            DepartmentId = x.DepartmentId,
            DepartmentFullName = x.Department != null ? x.Department.FullName : null,
            DepartmentOrderWeight = x.Department != null ? x.Department.DepartmentOrderWeight : null,
            Position = x.Position,
            PositionOrderWeight = x.PositionOrderWeight,
            Location = x.Location,
            RegisteredLocation = x.RegisteredLocations
                .Where(x => x.DateTo == null)
                .Select(x => x.RegisteredLocation!.Name)
                .First(),
            CurrentLocation = x.CurrentLocation,

            Comments = x.Comments,
            IsActive = x.IsActive,
            EnglishLevelId = x.EnglishLevelId,
            EnglishLevelName = x.EnglishLevel != null ? x.EnglishLevel.InternalName : null,
            EnglishLevelOrderWeight = x.EnglishLevel != null ? x.EnglishLevel.OrderWeight : null,

            GeneralSeniorityLevel = TeamboardDbFunctions.GetUserGeneralSeniority(x.UserId),

            SeniorityLevelId = x.UserSpecializations
                .Where(sp => sp.IsPrimary)
                .Select(sp => sp.SeniorityLevelId)
                .FirstOrDefault(),

            SeniorityLevel = x.UserSpecializations
                .Where(sp => sp.IsPrimary)
                .Where(sp => sp.SeniorityLevel != null)
                .Select(sp => sp.SeniorityLevel!.InternalName)
                .FirstOrDefault(),

            SeniorityLevelOrderWeight = x.UserSpecializations
                .Where(sp => sp.IsPrimary)
                .Where(sp => sp.SeniorityLevel != null)
                .Select(sp => (int?)sp.SeniorityLevel!.OrderWeight)
                .FirstOrDefault(),

            OdpNonPrimarySpecializations = x.UserSpecializations
                .AsQueryable()
                .Select(OdpNonPrimarySpecialization.MapNonPrimarySpecialization)
                .OrderByDescending(s => s.CreatedAtUtc)
                .ToList(),

            OdpUserPerformanceReviews = x.UserPerformanceReviews
                .AsQueryable()
                .Select(OdpUserPerformanceReview.MapUserPerformanceReview)
                .ToList()
        };
}
