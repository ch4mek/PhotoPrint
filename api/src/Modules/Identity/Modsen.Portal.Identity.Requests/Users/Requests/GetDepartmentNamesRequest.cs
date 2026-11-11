using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetDepartmentNamesRequest
    : IQueryHandler<GetDepartmentNamesRequest.Query, List<DepartmentWithNameOutput>>
{
    private readonly IIdentityDbContext _dbContext;

    public GetDepartmentNamesRequest(IIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<DepartmentWithNameOutput>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _dbContext.Departments
            .Select(department => new DepartmentWithNameOutput(department.Id, department.ShortName, department.FullName))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<DepartmentWithNameOutput>>;
}
