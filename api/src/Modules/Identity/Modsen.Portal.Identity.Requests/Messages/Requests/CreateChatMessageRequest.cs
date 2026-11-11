using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Agent.Contract.Models;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Messages.Core.Composers;
using Modsen.Portal.Identity.Requests.Messages.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Messages.Requests
{
    internal sealed class CreateChatMessageRequest : ICommandHandler<CreateChatMessageRequest.Command, List<MessageCoreDto>>
    {
        private readonly IAuthenticatedUserContext _authenticatedUserContext;
        private readonly IIdentityDbContext _identityDbContext;
        private readonly ICopilotService _copilotService;
        private readonly IUserDataComposer _userDataComposer;

        public CreateChatMessageRequest(
            IAuthenticatedUserContext authenticatedUserContext,
            IIdentityDbContext identityDbContext,
            ICopilotService copilotService,
            IUserDataComposer userDataComposer)
        {
            _authenticatedUserContext = authenticatedUserContext;
            _identityDbContext = identityDbContext;
            _copilotService = copilotService;
            _userDataComposer = userDataComposer;
        }

        public async Task<List<MessageCoreDto>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var userId = _authenticatedUserContext.GetRequiredUserId();
            var email = _authenticatedUserContext.UserEmail;

            var userData = email == null ? null
                : await _userDataComposer.ComposeAsync(email, cancellationToken);

            var conversationSessionEntity = await GetConversationSession(userId, cancellationToken);

            var session = conversationSessionEntity == null ? null
                : new ConversationSession
                {
                    ConversationId = conversationSessionEntity.ConversationId,
                    Token = conversationSessionEntity.Token,
                    ExpiresAtUtc = conversationSessionEntity.ExpiresAtUtc,
                    Watermark = conversationSessionEntity.Watermark,
                };

            var refreshSessionResult = await _copilotService.RefreshOrResumeConversationAsync(
                session, userData, userId, cancellationToken);

            session = refreshSessionResult.ConversationSession;

            if (refreshSessionResult.IsNewConversation)
            {
                conversationSessionEntity = new DigitalHrConversationSession
                {
                    ConversationId = session.ConversationId,
                    Token = session.Token,
                    ExpiresAtUtc = session.ExpiresAtUtc,
                    Watermark = session.Watermark,
                    UserId = userId,
                    CreatedAtUtc = DateTime.UtcNow,
                };

                _identityDbContext.DigitalHrConversationSessions.Add(conversationSessionEntity);
            }
            else
            {
                UpdateConversationSessionEntity(conversationSessionEntity!, session);
            }

            var messages = new List<UserMessage>
            {
                new()
                {
                    UserId = userId,
                    CreatedAtUtc = DateTime.UtcNow,
                    Message = request.Question,
                    Role = MessageRole.User,
                }
            };

            var response = await _copilotService.SendMessageAsync(session, userId, request.Question, cancellationToken);

            UpdateConversationSessionEntity(conversationSessionEntity!, session);

            if (response.Count == 0)
            {
                response = [PortalMessages.DigitalHrEmptyMessage];
            }

            var assistantMessages = response.Select(message => new UserMessage
            {
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow,
                Message = message,
                Role = MessageRole.Assistant,
            });

            messages.AddRange(assistantMessages);

            _identityDbContext.UserMessages.AddRange(messages);

            await _identityDbContext.SaveChanges(cancellationToken);

            return messages.ConvertAll(MessageCoreDto.Of);
        }

        private Task<DigitalHrConversationSession?> GetConversationSession(
            int userId,
            CancellationToken cancellationToken)
        {
            return _identityDbContext.DigitalHrConversationSessions
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private static void UpdateConversationSessionEntity(
            DigitalHrConversationSession entity,
            ConversationSession session)
        {
            entity.ConversationId = session.ConversationId;
            entity.Token = session.Token;
            entity.ExpiresAtUtc = session.ExpiresAtUtc;
            entity.Watermark = session.Watermark;
        }

        public sealed record Command(string Question) : ICommand<List<MessageCoreDto>>, ITransactionalRequest;

        internal sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Question)
                    .NotEmpty()
                    .LongSpaceRule();
            }
        }
    }
}
