using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.Loaders
{
    internal interface IConversationLoader
    {
        Task<ConversationCoreDto?> LoadAsync(int conversationId, CancellationToken cancellationToken);
    }

    internal class ConversationLoader : IConversationLoader
    {
        private readonly ITeamBoardDbContext _teamBoardDbContext;
        private readonly IGetPerformanceReviewByConversationIdDbQuery _getUserPerformanceReviewByConversationIdDbQuery;
        private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

        public ConversationLoader(
            ITeamBoardDbContext teamBoardDbContext,
            IGetPerformanceReviewByConversationIdDbQuery getUserPerformanceReviewByConversationIdDbQuery,
            ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
        {
            _teamBoardDbContext = teamBoardDbContext;
            _getUserPerformanceReviewByConversationIdDbQuery = getUserPerformanceReviewByConversationIdDbQuery;
            _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        }

        public async Task<ConversationCoreDto?> LoadAsync(int conversationId, CancellationToken cancellationToken)
        {
            var conversation = await _teamBoardDbContext.Conversations
                .Include(x => x.ConversationFiles)
                .Include(x => x.ConversationDocumentLinks)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);

            var performanceReviewAssignedToConversation =
                await _teamBoardDbContext.UserPerformanceReviews
                    .FirstOrDefaultAsync(x => x.ConversationId == conversationId, cancellationToken);

            if (conversation != null &&
                !await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(conversation.UserId, cancellationToken))
            {
                throw new ActionNotAllowedException();
            }

            if (conversation != null && performanceReviewAssignedToConversation != null)
            {
                var userPerformanceReview = await _getUserPerformanceReviewByConversationIdDbQuery.ExecuteAsync(conversationId, cancellationToken);

                return new ConversationCoreDto(
                    conversationId,
                    conversation.Date,
                    conversation.ConversationType.GetDescription(),
                    conversation.ConversationResult.ToString(),
                    conversation.Theses,
                    conversation.EmployeeTheses,
                    conversation.Agreements,
                    userPerformanceReview?.PerformanceReviewDate,
                    userPerformanceReview?.Status,
                    conversation.SupervisorId,
                    conversation.UserId,
                    performanceReviewAssignedToConversation.Id,
                    conversation.ConversationFiles.Select(x => new ConversationFileCoreDto(x.Id, x.FileId, x.FileName)).ToList(),
                    conversation.ConversationDocumentLinks.Select(x => new ConversationDocumentLinkCoreDto(x.Id, x.Link)).ToList());
            }

            return conversation == null
                ? null
                : new ConversationCoreDto(
                    conversationId,
                    conversation.Date,
                    conversation.ConversationType.GetDescription(),
                    conversation.ConversationResult.ToString(),
                    conversation.Theses,
                    conversation.EmployeeTheses,
                    conversation.Agreements,
                    null,
                    null,
                    conversation.SupervisorId,
                    conversation.UserId,
                    null,
                    conversation.ConversationFiles.Select(x => new ConversationFileCoreDto(x.Id, x.FileId, x.FileName)).ToList(),
                    conversation.ConversationDocumentLinks
                        .Select(x => new ConversationDocumentLinkCoreDto(x.Id, x.Link)).ToList()
                );
        }
    }
}
