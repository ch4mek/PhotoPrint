using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Core;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;
using Modsen.Portal.KnowledgeBase.Requests.Core;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;

internal sealed class UpdateArticleRequest : ICommandHandler<UpdateArticleRequest.Command, ArticleDto>
{
    private readonly IKnowledgeBaseDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly IArticleLoader _articleLoader;
    private readonly IUrlsFromTextExtractor _urlsFromTextExtractor;
    private readonly IIsArticleWithSameNameExistBusinessRule _isArticleWithSameNameExistBusinessRule;

    public UpdateArticleRequest(
        IKnowledgeBaseDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        IArticleLoader articleLoader,
        IUrlsFromTextExtractor urlsFromTextExtractor,
        IIsArticleWithSameNameExistBusinessRule isArticleWithSameNameExistBusinessRule)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _articleLoader = articleLoader;
        _urlsFromTextExtractor = urlsFromTextExtractor;
        _isArticleWithSameNameExistBusinessRule = isArticleWithSameNameExistBusinessRule;
    }

    public async Task<ArticleDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Articles
            .Include(x => x.ArticleTags)
            .Include(x => x.ArticleReferences)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
           ?? throw new EntityNotFoundException($"There is no article with Id {request.Id}");

        var isContain = await _isArticleWithSameNameExistBusinessRule.IsCorrespondToRuleAsync(
            request.GroupId, request.Name, cancellationToken, request.Id);

        if (isContain)
        {
            throw new InvalidDataAppException("Article with the same name already exists");
        }

        if (request.GroupId != entity.GroupId &&
           !await _dbContext.Groups
           .AnyAsync(x => x.Id == request.GroupId, cancellationToken))
        {
            throw new EntityConflictException("You can't add a group that doesn't exist");
        }

        entity.GroupId = request.GroupId;
        entity.Name = request.Name;
        entity.Icon = request.Icon;
        entity.Text = request.Text;
        entity.Keywords = request.Keywords;
        entity.IsActive = request.IsActive;
        entity.IsAvailableOnMainScreen = request.IsAvailableOnMainScreen;
        entity.UpdatedById = _authenticatedUserContext.UserId;
        entity.UpdatedAtUtc = _clockService.UtcNow;

        _dbContext.ArticleReferences.RemoveRange(entity.ArticleReferences);
        entity.ArticleReferences.Clear();
        entity.ArticleReferences.AddRange(GetReferenceEntities(request.Text));

        _dbContext.ArticleTags.RemoveRange(entity.ArticleTags);
        entity.ArticleTags.Clear();

        foreach (var t in request.ArticleTags ?? [])
        {
            var tag = await _dbContext.Tags.
                FirstOrDefaultAsync(i => i.Name == t, cancellationToken);

            if (tag == null)
            {
                tag = new() {  Name = t.ToLower(System.Globalization.CultureInfo.InvariantCulture) };
            }

            _dbContext.ArticleTags.Add(new() { Tag = tag, Article = entity });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var articleDto = await _articleLoader.LoadAsync(entity.Id, cancellationToken);

        return articleDto!;
    }

    private List<ArticleReferenceEntity> GetReferenceEntities(string text)
    {
        var references = _urlsFromTextExtractor.ExtractUrls(text);

        return references.Select(x => new ArticleReferenceEntity
        {
            ExternalUrl = x
        }).ToList();
    }

    internal sealed record Command(
        int Id,
        int GroupId,
        string Name,
        string? Icon,
        string Text,
        string Keywords,
        bool IsActive,
        bool IsAvailableOnMainScreen,
        List<string>? ArticleTags)
        : ICommand<ArticleDto>, ITransactionalRequest;

    internal sealed class UpdateArticleRequestValidator : AbstractValidator<Command>
    {
        public UpdateArticleRequestValidator()
        {
            RuleFor(command => command.Name)
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(250);

            RuleFor(command => command.Keywords)
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(250);
        }
    }
}
