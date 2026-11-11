using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;

public interface IPageableOdpLabBoardInternToPageableOdpLabBoardInternDtoMapper
{
    PageableListOfItems<OdpLabBoardInternDto> Map(PageableListOfItems<OdpLabBoardIntern> pageableLabBoardInterns);
}
internal class PageableOdpLabBoardInternToPageableOdpLabBoardInternDtoMapper : IPageableOdpLabBoardInternToPageableOdpLabBoardInternDtoMapper
{
    public PageableListOfItems<OdpLabBoardInternDto> Map(PageableListOfItems<OdpLabBoardIntern> pageableLabBoardInterns)
    {
        return new PageableListOfItems<OdpLabBoardInternDto>
        {
            Items = pageableLabBoardInterns.Items.Select(x => new OdpLabBoardInternDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = x.FullName,

                ResourceGroupId = x.ResourceGroupId,
                ResourceGroupName = x.ResourceGroupName,
                CurrentLocation = x.CurrentLocation,

                Internship = x.Internship != null
                    ? new OdpInternshipDetailDto
                    {
                        Id = x.Internship.Id,
                        UserId = x.Internship.UserId,
                        Specialization = x.Internship.Specialization,
                        ApprovedPoints = x.Internship.ApprovedPoints,
                        CompletedPoints = x.Internship.CompletedPoints,
                        EngineerStage = x.Internship.EngineerStage,
                        StartDate = x.Internship.StartDate,
                        PlannedEndDate = x.Internship.PlannedEndDate,
                        Type = x.Internship.Type,
                        TypeOrderWeight = x.Internship.TypeOrderWeight,
                        FinalResult = x.Internship.FinalResult,
                        FinalResultOrderWeight = x.Internship.FinalResultOrderWeight
                    }
                    : null,

                Comments = x.Comments,
                IsActive = x.IsActive,
                EnglishLevelId = x.EnglishLevelId,
                EnglishLevelName = x.EnglishLevelName,
                EnglishLevelOrderWeight = x.EnglishLevelOrderWeight,
            }).ToList(),
            Total = pageableLabBoardInterns.Total,
            Page = pageableLabBoardInterns.Page,
            PageSize = pageableLabBoardInterns.PageSize
        }; 
    }
}
