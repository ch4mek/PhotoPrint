using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Requests.Core;

internal interface IUserLoader
{
    Task<InquiriesUserDto?> GetUserByIdAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<InquiriesUserDto[]> GetUsersByIdsAsync(
        int[] userIds,
        CancellationToken cancellationToken);
}

internal class UserLoader : IUserLoader
{
    private readonly IInquiriesDbContext _context;

    public UserLoader(IInquiriesDbContext context)
    {
        _context = context;
    }

    public Task<InquiriesUserDto?> GetUserByIdAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        return GetQuery(userId).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<InquiriesUserDto[]> GetUsersByIdsAsync(
        int[] userIds,
        CancellationToken cancellationToken)
    {
        return GetQuery(userIds).ToArrayAsync(cancellationToken);
    }

    private IQueryable<InquiriesUserDto> GetQuery(params int[] userIds)
    {
        return _context.InquiriesUsers
            .Where(entity => userIds.Contains(entity.UserId))
            .AsNoTracking()
            .Select(user => new InquiriesUserDto
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DepartmentId = user.DepartmentId,
                RegisteredLocationId = user.RegisteredLocationId,
                ResourceGroupId = user.ResourceGroupId
            });
    }
}
