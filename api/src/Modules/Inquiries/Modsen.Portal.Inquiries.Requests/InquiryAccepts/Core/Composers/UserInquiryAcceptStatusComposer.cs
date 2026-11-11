using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;

internal interface IUserInquiryAcceptStatusComposer
{
    void Compose(UserInquiryAcceptEntity acceptEntity, int setById, UserInquiryAcceptStatus status);
    void Compose(UserInquiryAcceptEntity acceptEntity, UserInquiryAcceptStatus status);
    void Compose(ICollection<UserInquiryAcceptEntity> acceptEntities, UserInquiryAcceptStatus status);
}

internal class UserInquiryAcceptStatusComposer : IUserInquiryAcceptStatusComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;

    public UserInquiryAcceptStatusComposer(IAuthenticatedUserContext authenticatedUserContext, IClockService clockService)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
    }

    public void Compose(UserInquiryAcceptEntity acceptEntity, int setById, UserInquiryAcceptStatus status)
    {
        acceptEntity.Status = status;

        if (status == UserInquiryAcceptStatus.Accepted)
        {
            acceptEntity.AcceptedAtUtc = _clockService.UtcNow;
            acceptEntity.AcceptedById = setById;
        }
        else if (status == UserInquiryAcceptStatus.Rejected || status == UserInquiryAcceptStatus.Cancelled)
        {
            acceptEntity.RejectedAtUtc = _clockService.UtcNow;
            acceptEntity.RejectedById = setById;
        }
    }

    public void Compose(UserInquiryAcceptEntity acceptEntity, UserInquiryAcceptStatus status)
    {
        Compose(acceptEntity, _authenticatedUserContext.GetRequiredUserId(), status);   
    }

    public void Compose(ICollection<UserInquiryAcceptEntity> acceptEntities, UserInquiryAcceptStatus status)
    {
        if (status == UserInquiryAcceptStatus.Accepted)
        {
            foreach (var acceptEntity in acceptEntities)
            {
                acceptEntity.Status = status;
                acceptEntity.AcceptedAtUtc = _clockService.UtcNow;
                acceptEntity.AcceptedById = _authenticatedUserContext.UserId;
            }
        }
        else if (status == UserInquiryAcceptStatus.Rejected)
        {
            foreach (var acceptEntity in acceptEntities)
            {
                acceptEntity.Status = status;
                acceptEntity.RejectedAtUtc = _clockService.UtcNow;
                acceptEntity.RejectedById = _authenticatedUserContext.UserId;
            }
        }
        else
        {
            foreach(var acceptEntity in acceptEntities)
            {
                acceptEntity.Status = status;
            }
        }
    }
}
