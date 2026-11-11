using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface ISyncDepartmentDbQuery
{
    Task ExecuteAsync(SyncDepartmentDbQueryRequest request, CancellationToken cancellationToken);
}

internal class SyncDepartmentDbQueryRequest
{
    public required string DepartmentId { get; set; }
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
    public int DepartmentOrderWeight { get; set; }
    public required string DepartmentReportingToRoleName { get; set; }
}

internal class SyncDepartmentDbQuery : ISyncDepartmentDbQuery
{
    private readonly TeamBoardDbContext _teamBoardDbContext;

    public SyncDepartmentDbQuery(TeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task ExecuteAsync(SyncDepartmentDbQueryRequest request, CancellationToken cancellationToken)
    {
        var department = await _teamBoardDbContext.TeamBoardDepartments
            .FirstOrDefaultAsync(x => x.DepartmentId == request.DepartmentId, cancellationToken);
        if (department == null)
        {
            department = new TeamboardDepartment
            {
                DepartmentId = request.DepartmentId,
                FullName = request.FullName,
                ShortName = request.ShortName,
                DepartmentReportingToRoleName = request.DepartmentReportingToRoleName,
            };

            Update(request, department);
            _teamBoardDbContext.TeamBoardDepartments.Add(department);
        }
        else
        {
            Update(request, department);
            _teamBoardDbContext.TeamBoardDepartments.Update(department);
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Update(SyncDepartmentDbQueryRequest request, TeamboardDepartment department)
    {
        department.FullName = request.FullName;
        department.ShortName = request.ShortName;
        department.DepartmentOrderWeight = request.DepartmentOrderWeight;
        department.DepartmentReportingToRoleName = request.DepartmentReportingToRoleName;
    }
}
