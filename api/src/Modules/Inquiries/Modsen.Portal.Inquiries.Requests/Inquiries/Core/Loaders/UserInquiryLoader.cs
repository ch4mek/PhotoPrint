using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;

internal interface IUserInquiryLoader
{
    Task<UserInquiryCoreDto?> Load(int inquiryId, CancellationToken cancellationToken);
    Task<List<UserInquiryCoreDto>> Load(int[] inquiryIds, CancellationToken cancellationToken);
}

internal class UserInquiryLoader : IUserInquiryLoader
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IIdentityContract _identityContract;

    public UserInquiryLoader(
        IInquiriesDbContext inquiriesDbContext,
        IIdentityContract identityContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _identityContract = identityContract;
    }

    public async Task<UserInquiryCoreDto?> Load(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiries = await Load([inquiryId], cancellationToken);
        return inquiries.FirstOrDefault();
    }

    public async Task<List<UserInquiryCoreDto>> Load(int[] inquiryIds, CancellationToken cancellationToken)
    {
        var inquiries = await _inquiriesDbContext.UserInquiries
            .Where(x => inquiryIds.Contains(x.Id))
            .Include(x => x.UserInquiryDates)
            .ThenInclude(d => d.CalendarExclusion)
            .Include(x => x.UserInquiryAccepts)
            .AsSplitQuery()
            .Select(UserInquiryCoreDto.Map)
            .ToListAsync(cancellationToken);

        var usersInquiryFor = inquiries.Select(x => x.CreatedForUser.Id).ToArray();
        var usersWithManagers = await _identityContract.GetUsersWithManagersAsync(usersInquiryFor, cancellationToken);
        var userIdManagerIdMap = new Dictionary<int, int>();

        foreach (var userWithManagers in usersWithManagers)
        {
            var managerId = userWithManagers.ChiefId;
            if (userWithManagers.GroupManagerId != 0)
            {
                managerId = userWithManagers.GroupManagerId;
            }
            else if (userWithManagers.HeadOfDepartmentUserId != 0)
            {
                managerId = userWithManagers.HeadOfDepartmentUserId;
            }

            userIdManagerIdMap[userWithManagers.UserId] = managerId;
        }

        var managers = await _inquiriesDbContext.InquiriesUsers
            .AsNoTracking()
            .Where(x => userIdManagerIdMap.Values.Contains(x.UserId))
            .ToDictionaryAsync(x => x.UserId, cancellationToken);

        foreach (var inquiry in inquiries)
        {
            if (inquiry.ManagerOfUser == null)
            {
                var managerId = userIdManagerIdMap[inquiry.CreatedForUser!.Id];
                var manager = managerId == 0
                    ? null 
                    : managers[managerId];

                inquiry.ManagerOfUser = manager == null
                    ? null
                    : new InquiriesUserDto
                    {
                        Id = manager.UserId,
                        FirstName = manager.FirstName,
                        LastName = manager.LastName,
                        Email = manager.Email,
                        DepartmentId = manager.DepartmentId,
                        ResourceGroupId = manager.ResourceGroupId,
                        RegisteredLocationId = manager.RegisteredLocationId,
                        AvatarFileId = manager.AvatarFileId,
                        Color = manager.Color,
                        Position = manager.Position
                    };
            }
        }

        return inquiries;
    }
}
