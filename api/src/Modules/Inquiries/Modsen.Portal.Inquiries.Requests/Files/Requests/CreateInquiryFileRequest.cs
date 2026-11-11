using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Files.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Files.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Files.Requests;

internal sealed class CreateInquiryFileRequest : ICommandHandler<CreateInquiryFileRequest.Command, InquiryFileDto>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IHasUserAccessToFilesBusinessRule _hasUserAccessToFilesBusinessRule;

    public CreateInquiryFileRequest(
        IInquiriesDbContext inquiriesDbContext,
        IHasUserAccessToFilesBusinessRule hasUserAccessToFilesBusinessRule)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _hasUserAccessToFilesBusinessRule = hasUserAccessToFilesBusinessRule;
    }

    public async Task<InquiryFileDto> Handle(Command request, CancellationToken cancellationToken)
    {
        _ = await _inquiriesDbContext.UserInquiries.FirstOrDefaultAsync(x => x.Id == request.InquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"Inquiry with Id {request.InquiryId} doesn't exist.");

        if (!await _hasUserAccessToFilesBusinessRule.IsCorrespondsToBusiness(request.InquiryId, cancellationToken))
        {
            throw new ActionNotAllowedException($"You can't add a file for inquiry: {request.InquiryId}");
        }

        var inquiryFileEntity = new UserInquiryFileEntity
        {
            UserInquiryId = request.InquiryId,
            FileId = request.FileId
        };

        _inquiriesDbContext.UserInquiryFiles.Add(inquiryFileEntity);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return new InquiryFileDto
        {
            Id = inquiryFileEntity.Id,
            FileId = inquiryFileEntity.FileId,
            CreatedById = inquiryFileEntity.CreatedById,
            CreatedAtUtc = inquiryFileEntity.CreatedAtUtc
        };
    }

    internal sealed record Command(int InquiryId, string FileId) : ICommand<InquiryFileDto>, ITransactionalRequest;
}
