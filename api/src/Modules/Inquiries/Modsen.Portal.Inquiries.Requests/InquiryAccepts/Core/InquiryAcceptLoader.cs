using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core;

internal interface IInquiryAcceptLoader
{
    Task<UserInquiryAcceptCoreDto> Load(int acceptId, CancellationToken cancellationToken);
    Task<List<UserInquiryAcceptCoreDto>> Load(int[] acceptIds, CancellationToken cancellationToken);
}

internal class InquiryAcceptLoader : IInquiryAcceptLoader
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public InquiryAcceptLoader(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<UserInquiryAcceptCoreDto> Load(int acceptId, CancellationToken cancellationToken)
    {
        var inquiries = await Load([acceptId], cancellationToken);
        return inquiries.FirstOrDefault()
            ?? throw new EntityNotFoundException($"Inquiry accept by id {acceptId} not found");
    }

    public async Task<List<UserInquiryAcceptCoreDto>> Load(int[] acceptIds, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserInquiryAccepts
            .Where(x => acceptIds.Contains(x.Id))
            .Select(x => new UserInquiryAcceptCoreDto
            {
                Id = x.Id,
                Status = x.Status,
                AcceptPolicyScope = x.AcceptPolicyScope,
                AcceptUserPosition = x.AcceptUserPosition,
                AcceptUser = x.AcceptUser == null
                ? null
                : new InquiriesUserDto
                {
                    Id = x.AcceptUser.UserId,
                    FirstName = x.AcceptUser.FirstName,
                    LastName = x.AcceptUser.LastName,
                    Email = x.AcceptUser.Email,
                    DepartmentId = x.AcceptUser.DepartmentId,
                    AvatarFileId = x.AcceptUser.AvatarFileId,
                    Position = x.AcceptUser.Position
                },
                AcceptedByUser = x.AcceptedByUser == null
                ? null
                : new InquiriesUserDto
                {
                    Id = x.AcceptedByUser.UserId,
                    FirstName = x.AcceptedByUser.FirstName,
                    LastName = x.AcceptedByUser.LastName,
                    Email = x.AcceptedByUser.Email,
                    DepartmentId = x.AcceptedByUser.DepartmentId,
                    AvatarFileId = x.AcceptedByUser.AvatarFileId,
                    Position = x.AcceptedByUser.Position
                },
                AcceptedAtUtc = x.AcceptedAtUtc,
                RejectedByUser = x.RejectedByUser == null
                ? null
                : new InquiriesUserDto
                {
                    Id = x.RejectedByUser.UserId,
                    FirstName = x.RejectedByUser.FirstName,
                    LastName = x.RejectedByUser.LastName,
                    Email = x.RejectedByUser.Email,
                    DepartmentId = x.RejectedByUser.DepartmentId,
                    AvatarFileId = x.RejectedByUser.AvatarFileId,
                    Position = x.RejectedByUser.Position
                },
                RejectedAtUtc = x.RejectedAtUtc
            }).ToListAsync(cancellationToken);
    }
}
