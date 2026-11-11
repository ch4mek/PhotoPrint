using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Agent.Contract.Models;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Messages.Core.Composers;
using Modsen.Portal.Identity.Requests.Messages.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Messages.Core.Loader;

namespace Modsen.Portal.Identity.Requests.Messages.Requests
{
    internal sealed class GetChatHistoryRequest : ICommandHandler<GetChatHistoryRequest.Command, PageableListOfItems<MessageCoreDto>>
    {
        private readonly IAuthenticatedUserContext _authenticatedUserContext;
        private readonly IIdentityDbContext _identityDbContext;
        private readonly IUserMessageLoader _iUserMessageLoader;
        private readonly ICopilotService _copilotService;
        private readonly IUserDataComposer _userDataComposer;

        public GetChatHistoryRequest(
            IAuthenticatedUserContext authenticatedUserContext,
            IIdentityDbContext identityDbContext,
            IUserMessageLoader iUserMessageLoader,
            ICopilotService copilotService,
            IUserDataComposer userDataComposer)
        {
            _authenticatedUserContext = authenticatedUserContext;
            _identityDbContext = identityDbContext;
            _iUserMessageLoader = iUserMessageLoader;
            _copilotService = copilotService;
            _userDataComposer = userDataComposer;
        }
        public async Task<PageableListOfItems<MessageCoreDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userEmail = _authenticatedUserContext.UserEmail!;
            var userId = _authenticatedUserContext.GetRequiredUserId();

            var userData = await _userDataComposer.ComposeAsync(userEmail, cancellationToken);
            var conversation = await GetConversationSessions(userId, cancellationToken);

            var refreshSessionResult = await _copilotService.RefreshOrResumeConversationAsync(
                conversation == null ? null : new ConversationSession
                {
                    ConversationId = conversation.ConversationId,
                    Token = conversation.Token,
                    ExpiresAtUtc = conversation.ExpiresAtUtc,
                    Watermark = conversation.Watermark,
                }, userData, userId, cancellationToken);

            if (refreshSessionResult.IsNewConversation)
            {
                conversation = new DigitalHrConversationSession
                {
                    ConversationId = refreshSessionResult.ConversationSession.ConversationId,
                    Token = refreshSessionResult.ConversationSession.Token,
                    ExpiresAtUtc = refreshSessionResult.ConversationSession.ExpiresAtUtc,
                    Watermark = refreshSessionResult.ConversationSession.Watermark,
                    UserId = userId,
                };

                _identityDbContext.DigitalHrConversationSessions.Add(conversation);
                conversation.Watermark = refreshSessionResult.ConversationSession.Watermark;

                _identityDbContext.UserMessages.AddRange((refreshSessionResult.messages ?? [])
                    .Select(m => new UserMessage
                    {
                        UserId = userId,
                        CreatedAtUtc = DateTime.UtcNow,
                        Message = m,
                        Role = MessageRole.Assistant,
                    }));

                await _identityDbContext.SaveChanges(cancellationToken);
            }

            var result = await _iUserMessageLoader.LoadAsync(userId, request.QueryOptions, cancellationToken);

            return result;
        }

        private Task<DigitalHrConversationSession?> GetConversationSessions(int userId, CancellationToken cancellationToken)
        {
            return _identityDbContext.DigitalHrConversationSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public sealed record Command(ODataQueryOptions<MessageCoreDto> QueryOptions)
            : ICommand<PageableListOfItems<MessageCoreDto>>, ITransactionalRequest;
    }
}
