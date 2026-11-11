using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Home.Core.Converters;

internal interface ITeamboardUserToDepartmentNameConverter
{
    string GetDepartmentName(TeamBoardUser user);
}

internal class TeamboardUserToDepartmentNameConverter :  ITeamboardUserToDepartmentNameConverter
{
    public string GetDepartmentName(TeamBoardUser user)
    {
        if (user.Position.IsNullOrEmpty())
        {
            return string.Empty;
        }

        if (user.Position.IsCto() || user.Position.IsCeo())
        {
            return user.Position;
        }

        if ((user.Department?.FullName).IsNullOrEmpty())
        {
            return string.Empty;
        }

        if (user.Position.IsEmployee() || user.Position.IsGroupManager() || user.Position.IsSystemAdministrator())
        {
            return user.Department.FullName;
        }

        if (user.Position.IsHeadOfDepartment())
        {
            return $"Head of {user.Department.FullName}";
        }

        return string.Empty;
    }
}
