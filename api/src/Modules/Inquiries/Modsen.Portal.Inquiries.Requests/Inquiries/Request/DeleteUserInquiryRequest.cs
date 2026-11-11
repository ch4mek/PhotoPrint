using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Loaders;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class DeleteUserInquiryRequest : ICommandHandler<DeleteUserInquiryRequest.Command, UserInquiryCoreDto>
{
    private readonly IUserInquiryLoader _userInquiryLoader;
    private readonly IUserLoader _userLoader;
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IHasUserAccessToMutateInquiryBusinessRule _hasUserAccessToMutateInquiryBusinessRule;

    public DeleteUserInquiryRequest(
        IUserInquiryLoader userInquiryLoader,
        IUserLoader userLoader,
        IInquiriesDbContext inquiriesDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IHasUserAccessToMutateInquiryBusinessRule hasUserAccessToMutateInquiryBusinessRule)
    {
        _userInquiryLoader = userInquiryLoader;
        _userLoader = userLoader;
        _inquiriesDbContext = inquiriesDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _hasUserAccessToMutateInquiryBusinessRule = hasUserAccessToMutateInquiryBusinessRule;
    }

    public async Task<UserInquiryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var userInquiry = await _inquiriesDbContext.UserInquiries
            .Include(x => x.UserInquiryAccepts)
            .FirstOrDefaultAsync(x => x.Id == request.InquiryId, cancellationToken)
            ?? throw new EntityNotFoundException($"User inquiry by id {request.InquiryId} not found");

        if (userInquiry.Status != UserInquiryStatus.Open)
        {
            throw new ActionNotAllowedException($"User inquiry by id {request.InquiryId} was approved or rejected or cancelled");
        }

        if (!await _hasUserAccessToMutateInquiryBusinessRule.IsCorrespondsToBusiness(userInquiry.UserId, cancellationToken))
        {
            throw new ActionNotAllowedException($"You can't delete inquiry for user: {userInquiry.UserId}");
        }

        var user = await GetUser(userInquiry.UserId, userInquiry.Id, cancellationToken);

        CreateCancelledMessage(user, userInquiry.Id);

        var inquiryAcceptsForDeleting = userInquiry.UserInquiryAccepts
            .Where(x => x.Status != UserInquiryAcceptStatus.Accepted);

        _inquiriesDbContext.UserInquiryAccepts.RemoveRange(inquiryAcceptsForDeleting);

        userInquiry.UserInquiryAccepts.RemoveAll(x => x.Status != UserInquiryAcceptStatus.Accepted);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return await GetUserInquiry(request.InquiryId, cancellationToken);
    }

    private async Task<InquiriesUserDto> GetUser(int userId, int inquiryId, CancellationToken cancellationToken)
    {
        var authenticatedUserId = _authenticatedUserContext.UserId
            ?? throw new EntityNotFoundException($"User by id {_authenticatedUserContext.UserId} not found");

        if (userId != authenticatedUserId)
        {
            throw new ActionNotAllowedException($"You don't have pemission to update user inquiry by id {inquiryId}");
        }

        return await _userLoader.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException($"User by id {userId} not found");
    }

    private void CreateCancelledMessage(InquiriesUserDto user, int inquiryId)
    {
        _inquiriesDbContext.UserInquiryComments.Add(new UserInquiryCommentEntity
        {
            Comment = $"Request has been cancelled by {user.LastName} {user.FirstName}",
            UserInquiryId = inquiryId
        });
    }

    private async Task<UserInquiryCoreDto> GetUserInquiry(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await _userInquiryLoader.Load(inquiryId, cancellationToken);
        return inquiry ?? throw new EntityNotFoundException($"Inquiry by id {inquiryId} not found");
    }

    internal sealed record Command(int InquiryId) : ICommand<UserInquiryCoreDto>, ITransactionalRequest;
}
