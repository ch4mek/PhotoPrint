using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Comments.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Requests.Comments.Core;

internal interface IInquiryCommentsLoader
{
    Task<InquiryCommentCoreDto> Load(int commentId, CancellationToken cancellationToken);
    Task<List<InquiryCommentCoreDto>> Load(int[] commentIds, CancellationToken cancellationToken);
}

internal class InquiryCommentsLoader : IInquiryCommentsLoader
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public InquiryCommentsLoader(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<InquiryCommentCoreDto> Load(int commentId, CancellationToken cancellationToken)
    {
        var comments = await Load([commentId], cancellationToken);
        return comments.FirstOrDefault()
            ?? throw new EntityNotFoundException($"Comment by id {commentId} not found");
    }

    public Task<List<InquiryCommentCoreDto>> Load(int[] commentIds, CancellationToken cancellationToken)
    {
        return _inquiriesDbContext.UserInquiryComments
            .Where(x => commentIds.Contains(x.Id))
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new InquiryCommentCoreDto
            {
                Id = x.Id,
                Comment = x.Comment,
                CreatedByUser = x.CreatedByUser == null
                   ? null
                   : new InquiriesUserDto
                   {
                       Id = x.CreatedByUser.UserId,
                       FirstName = x.CreatedByUser.FirstName,
                       LastName = x.CreatedByUser.LastName,
                       Email = x.CreatedByUser.Email,
                       DepartmentId = x.CreatedByUser.DepartmentId,
                       AvatarFileId = x.CreatedByUser.AvatarFileId,
                       Position = x.CreatedByUser.Position
                   },
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedByUser = x.UpdatedByUser == null
                   ? null
                   : new InquiriesUserDto
                   {
                       Id = x.UpdatedByUser.UserId,
                       FirstName = x.UpdatedByUser.FirstName,
                       LastName = x.UpdatedByUser.LastName,
                       Email = x.UpdatedByUser.Email,
                       DepartmentId = x.UpdatedByUser.DepartmentId,
                       AvatarFileId = x.UpdatedByUser.AvatarFileId,
                       Position = x.UpdatedByUser.Position
                   },
                UpdatedAtUtc = x.UpdatedAtUtc,
                Files = x.UserInquiryCommentFiles
                    .Select(f => new InquiryCommentFileCoreDto
                    {
                        Id = f.Id,
                        FileId = f.FileId,
                        FileName = f.FileName,
                        FileSize = f.FileSize
                    }).ToList(),
            }).ToListAsync(cancellationToken);
    }
}
