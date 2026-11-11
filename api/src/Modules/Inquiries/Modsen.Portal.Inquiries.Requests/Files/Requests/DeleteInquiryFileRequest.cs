using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Files.Requests;

internal sealed class DeleteInquiryFileRequest : ICommandHandler<DeleteInquiryFileRequest.Command, int>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public DeleteInquiryFileRequest(
        IInquiriesDbContext inquiriesDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var inquiryFileEntity = await _inquiriesDbContext.UserInquiryFiles
           .FirstOrDefaultAsync(x => x.UserInquiryId == request.InquiryId
           && x.Id == request.Id, cancellationToken)
           ?? throw new EntityNotFoundException($"File by id {request.Id} in inquiry with id {request.InquiryId} not found");

        if (inquiryFileEntity.CreatedById != _authenticatedUserContext.UserId)
        {
            throw new ActionNotAllowedException($"You can't delete a file: {request.Id}");
        }

        _inquiriesDbContext.UserInquiryFiles.Remove(inquiryFileEntity);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    internal sealed record Command(int InquiryId, int Id) : ICommand<int>, ITransactionalRequest;
}
