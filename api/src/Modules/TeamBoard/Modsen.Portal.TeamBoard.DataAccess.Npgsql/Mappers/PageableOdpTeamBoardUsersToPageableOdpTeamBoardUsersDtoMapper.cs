using Modsen.Portal.Core;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;

public interface IPageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper
{
    PageableListOfItems<OdpTeamBoardUserDto> Map(PageableListOfItems<OdpTeamBoardUser> pageableTeamBoardUsers);
}

internal class PageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper : IPageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper
{
    public PageableListOfItems<OdpTeamBoardUserDto> Map(PageableListOfItems<OdpTeamBoardUser> pageableTeamBoardUsers)
    {
        return new PageableListOfItems<OdpTeamBoardUserDto>
        {
            Items = pageableTeamBoardUsers.Items.Select(x => new OdpTeamBoardUserDto
            {
                Id = x.Id,
                OdpNonPrimarySpecializations = x.OdpNonPrimarySpecializations.Select(y =>
                    new OdpNonPrimarySpecializationDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        IsActive = y.IsActive,
                        SeniorityLevel = y.SeniorityLevel,
                        OrderWeight = y.OrderWeight,
                        CreatedAtUtc = y.CreatedAtUtc,
                        UserId = y.UserId,
                        IsVisibleSpecialization = y.IsVisibleSpecialization,
                        SeniorityLevelId = y.SeniorityLevelId
                    }
                ).ToList(),
                IsActive = x.IsActive,
                SeniorityLevel = x.SeniorityLevel,
                SeniorityLevelId = x.SeniorityLevelId,
                GeneralSeniorityLevel = x.GeneralSeniorityLevel,
                Comments = x.Comments,
                Location = Enum.TryParse(x.Location, out LocationType location) ? location.GetDescription() : null,
                RegisteredLocation = x.RegisteredLocation,
                CurrentLocation = x.CurrentLocation,
                Position = x.Position,
                PositionOrderWeight = x.PositionOrderWeight,
                DepartmentId = x.DepartmentId,
                DepartmentFullName = x.DepartmentFullName,
                DepartmentOrderWeight = x.DepartmentOrderWeight,
                ResourceGroupId = x.ResourceGroupId,
                ResourceGroupName = x.ResourceGroupName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = x.FullName,
                ProfExperienceInMonths = CalculateDifferenceInMonthsBetweenDates(x.CareerStartedAt, DateOnly.FromDateTime(DateTime.UtcNow)),
                CareerStartedAt = x.CareerStartedAt,
                EnglishLevelId = x.EnglishLevelId,
                EnglishLevelName = x.EnglishLevelName,
                EnglishLevelOrderWeight = x.EnglishLevelOrderWeight,
                PrimarySpecializationId = x.PrimarySpecializationId,
                PrimarySpecializationName = x.PrimarySpecializationName,
                SeniorityLevelOrderWeight = x.SeniorityLevelOrderWeight
            }).ToList(),
            Total = pageableTeamBoardUsers.Total,
            Page = pageableTeamBoardUsers.Page,
            PageSize = pageableTeamBoardUsers.PageSize
        };
    }

    private static int CalculateDifferenceInMonthsBetweenDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate == null || endDate == null)
        {
            return 0;
        }

        var yearsDifference = endDate.Value.Year - startDate.Value.Year;
        var monthsDifference = endDate.Value.Month - startDate.Value.Month;
        var totalMonths = yearsDifference * 12 + monthsDifference;

        if (endDate.Value.Day < startDate.Value.Day)
        {
            totalMonths--;
        }

        return totalMonths;
    }
}
