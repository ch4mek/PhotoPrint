namespace Modsen.Portal.Core.Inquiries;

public enum UserInquiryAcceptStatus
{
    Pending = 0,            // Waiting in queue after the current one, cannot be approved or rejected until its turn comes - "pending" from documentation
    WaitingForApprove = 1,  // Currently first in queue (being processed) - "pending" from documentation  
    Accepted = 2,
    Rejected = 3,
    Cancelled = 4           // Was cancelled because a previous item in the queue was rejected - "disabled pending" from documentation
}
