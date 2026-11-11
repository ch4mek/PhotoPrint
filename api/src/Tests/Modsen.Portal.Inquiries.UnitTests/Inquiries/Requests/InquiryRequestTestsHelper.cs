using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Requests;

internal static class InquiryRequestTestsHelper
{
    internal static int AuthenticatedUserId = 1;
    internal static void SetupUserContext(this Mock<IAuthenticatedUserContext> userContextMock, bool hasManageRequests, bool hasManageAllRequests)
    { 
        userContextMock.Setup(x => x.HasClaim(RequestsClaimTypeNames.ManageRequests.Name))
            .Returns(hasManageRequests);
        userContextMock.Setup(x => x.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
            .Returns(hasManageAllRequests);
        userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        userContextMock.Setup(x => x.UserId).Returns(AuthenticatedUserId);
    }
}
