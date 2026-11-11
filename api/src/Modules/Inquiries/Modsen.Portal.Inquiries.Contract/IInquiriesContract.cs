using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Contract.Models;

namespace Modsen.Portal.Inquiries.Contract;

public interface IInquiriesContract
{
    Task<bool> IsUserApproverOrSubstitute(int userId, CancellationToken cancellationToken);
    Task<List<UserInquiryDto>> GetUserInquiriesAsync(int userId, CancellationToken cancellationToken);
    Task<List<UserInquiryDto>> GetUsersInquiriesAsync(int[] userIds, CancellationToken cancellationToken, InquiryType? type = null, UserInquiryStatus? status = null);
    Task<List<UserBalanceDaysDto>> GetUserBalanceDays(int userId, CancellationToken cancellationToken);
    Task<List<CalendarExclusionDto>> GetCalendarExclusionAsync(int year, CancellationToken cancellationToken, int? countryId = null);
}
