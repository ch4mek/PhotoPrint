using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.AuthenticationFilters
{
    internal interface IConversationsAuthenticationFilter
    {
        IQueryable<Conversation> Apply(IQueryable<Conversation> conversations);
    }

    internal class ConversationsAuthenticationFilter : IConversationsAuthenticationFilter
    {
        private readonly IAuthenticatedUserContext _authenticatedUserContext;

        public ConversationsAuthenticationFilter(IAuthenticatedUserContext authenticatedUserContext)
        {
            _authenticatedUserContext = authenticatedUserContext;
        }

        public IQueryable<Conversation> Apply(IQueryable<Conversation> conversations)
        {
            var isHR = _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name);
            var isManageAllInterns = _authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name);
            var isManageInterns = _authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageInterns.Name);
            var currentUserRole = _authenticatedUserContext.GetRoles();

            conversations = conversations
                .Where(x => x.UserId != _authenticatedUserContext.GetRequiredUserId());

            if (currentUserRole.IsHeadOfDepartment())
            {
                conversations = conversations
                    .Where(x => x.User != null)
                    .Where(x => x.User!.DepartmentId == _authenticatedUserContext.DepartmentId ||
                    isHR && x.ConversationType == ConversationType.HRConversation ||
                    isManageAllInterns && x.User!.Position == PortalRoleNames.Intern);
            }
            else if (currentUserRole.IsGroupManager())
            {
                conversations = conversations
                    .Where(x => x.User != null)
                    .Where(x => x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId ||
                    isHR && x.ConversationType == ConversationType.HRConversation ||
                    isManageInterns && x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId && x.User!.Position == PortalRoleNames.Intern ||
                    isManageAllInterns && x.User!.Position == PortalRoleNames.Intern);
            }
            else if (currentUserRole.IsEmployee() || currentUserRole.IsSystemAdministrator())
            {
                conversations = conversations
                    .Where(x => x.User != null)
                    .Where(x => isHR && x.ConversationType == ConversationType.HRConversation ||
                    isManageAllInterns && x.User!.Position == PortalRoleNames.Intern);
            }

            return conversations;
        }
    }
}
