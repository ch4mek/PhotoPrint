using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpLabBoardIntern
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public OdpInternshipDetail? Internship { get; set; }

    public string? EnglishLevelId { get; set; }
    public string? EnglishLevelName { get; set; }
    public int? EnglishLevelOrderWeight { get; set; }

    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }

    public string? CurrentLocation { get; set; }
    public string? Comments { get; set; }

    public bool IsActive { get; set; }

    internal static readonly Expression<Func<TeamBoardUser, OdpLabBoardIntern>> MapTeamBoardUser =
        x => new OdpLabBoardIntern
        {
            Id = x.UserId,
            FirstName = x.FirstName,
            LastName = x.LastName,

            FullName = x.LastNameRu == null && x.FirstNameRu == null
                ? x.LastName + " " + x.FirstName
                : x.LastNameRu != null && x.FirstNameRu != null
                    ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                    : (x.LastNameRu ?? x.FirstNameRu) + " (" + x.LastName + " " + x.FirstName + ")",

            ResourceGroupId = x.ResourceGroupId,
            ResourceGroupName = x.ResourceGroup != null ? x.ResourceGroup.ResourceGroupName : null,
            CurrentLocation = x.CurrentLocation,

            Internship = x.InternshipDetails
            .OrderByDescending(x => x.StartDate)
            .Select(x => new OdpInternshipDetail
            {
                Id = x.Id,
                UserId = x.UserId,
                Specialization = x.Specialization,
                ApprovedPoints = x.ApprovedPoints,
                CompletedPoints = x.CompletedPoints,
                EngineerStage = x.EngineerStage,
                StartDate = x.StartDate,
                PlannedEndDate = x.PlannedEndDate,
                Type = x.Type,
                TypeOrderWeight = x.TypeOrderWeight,
                FinalResult = x.FinalResult,
                FinalResultOrderWeight = x.FinalResultOrderWeight
            })
            .FirstOrDefault(),

            Comments = x.Comments,
            IsActive = x.IsActive,
            EnglishLevelId = x.EnglishLevelId,
            EnglishLevelName = x.EnglishLevel != null ? x.EnglishLevel.InternalName : null,
            EnglishLevelOrderWeight = x.EnglishLevel != null ? x.EnglishLevel.OrderWeight : null,
        };
}
