using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Core;

internal interface IUserLoader
{
    Task<UserDto> LoadAsync(int userId, CancellationToken cancellationToken);
    Task<List<UserDto>> LoadAsync(int[] userIds, CancellationToken cancellationToken);
}

internal class UserLoader : IUserLoader
{
    private readonly IIdentityDbContext _dbContext;

    public UserLoader(IIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UserDto>> LoadAsync(int[] userIds, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Where(x => userIds.Contains(x.Id))
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Include(x => x.ResourceGroup)
            .AsSplitQuery()
            .Select(x => new UserDto
            {
                Id = x.Id,
                Color = x.Color,
                Email = x.Email!,
                AvatarColor = x.AvatarColor,
                AvatarFileId = x.AvatarFileId,
                TimeZone = x.TimeZone,
                DepartmentId = x.DepartmentId,

                ResourceGroupId = x.ResourceGroupId,
                ResourceGroupName = x.ResourceGroup == null || !x.ResourceGroup.IsActive
                ? null
                : x.ResourceGroup.Name,

                CountryId = x.CountryId,
                RegisteredLocationId = x.RegisteredLocationId,
                Location = x.Location,
                CurrentLocation = x.CurrentLocation,

                HireDate = x.HireDate,

                IsActive = x.IsActive,

                LastName = x.LastName,
                FirstName = x.FirstName,
                FullName = ToFullName.Reverse(x.FirstName, x.LastName),

                LastNameRu = x.LastNameRu,
                FirstNameRu = x.FirstNameRu,
                FullNameRu = x.LastNameRu == null && x.FirstNameRu == null ?
                    null :
                    ToFullName.Reverse(x.FirstNameRu, x.LastNameRu).Trim(),
                PatronymicRu = x.PatronymicRu,

                Signature = new UserSignatureDto
                {
                    Phone = x.SignaturePhone,
                    FullName = x.SignatureFullName,
                    Position = x.SignaturePosition,
                },

                RoleNames = x.UserRoles
                    .Select(f => f.Role == null ? null : f.Role.Name)
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .ToList()!,

                PositionPublicName = x.UserRoles
                    .Select(f => f.Role == null ? null : f.Role.PublicName)
                    .FirstOrDefault(),

                PositionOrderWeight = x.UserRoles
                    .Select(f => f.Role == null ? (int?)null : f.Role.OrderWeight)
                    .FirstOrDefault(),

                CreatedById = x.CreatedById
            }).ToListAsync(cancellationToken);
    }

    public async Task<UserDto> LoadAsync(int userId, CancellationToken cancellationToken)
    {
        var users = await LoadAsync([userId], cancellationToken);
        return users.FirstOrDefault() ??
            throw new EntityNotFoundException($"User with id {userId} does not exist");
    }
}
