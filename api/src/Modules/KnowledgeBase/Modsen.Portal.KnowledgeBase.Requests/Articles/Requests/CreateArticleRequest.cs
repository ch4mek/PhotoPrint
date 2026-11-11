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
using Modsen.Portal.KnowledgeBase.Requests.Core.Calculators;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;

internal sealed class CreateArticleRequest : ICommandHandler<CreateArticleRequest.Command, ArticleDto>
{
    private readonly IKnowledgeBaseDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly IArticleLoader _articleLoader;
    private readonly IUrlsFromTextExtractor _urlsFromTextExtractor;
    private readonly IMaxOrderWeightCalculator _maxOrderWeightCalculator;
    private readonly IIsArticleWithSameNameExistBusinessRule _isArticleWithSameNameExistBusinessRule;

    public CreateArticleRequest(
        IKnowledgeBaseDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        IArticleLoader articleLoader,
        IUrlsFromTextExtractor urlsFromTextExtractor,
        IMaxOrderWeightCalculator maxOrderWeightCalculator,
        IIsArticleWithSameNameExistBusinessRule isArticleWithSameNameExistBusinessRule)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _articleLoader = articleLoader;
        _urlsFromTextExtractor = urlsFromTextExtractor;
        _maxOrderWeightCalculator = maxOrderWeightCalculator;
        _isArticleWithSameNameExistBusinessRule = isArticleWithSameNameExistBusinessRule;
    }

    public async Task<ArticleDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var groupEntity = await _dbContext.Groups.FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken)
            ?? throw new EntityNotFoundException($"Group with Id {request.GroupId} doesn't exist.");

        var isContain = await _isArticleWithSameNameExistBusinessRule.IsCorrespondToRuleAsync(
            request.GroupId, request.Name, cancellationToken);

        if (isContain)
        {
            throw new InvalidDataAppException("Article with the same name already exists");
        }

        var article = new ArticleEntity
        {
            GroupId = request.GroupId,
            Group = groupEntity,
            Name = request.Name,
            Icon = request.Icon,
            Text = request.Text,
            Keywords = request.Keywords,
            IsActive = request.IsActive,
            IsAvailableOnMainScreen = request.IsAvailableOnMainScreen,
            OrderWeight = await _maxOrderWeightCalculator.CalculateAsync(request.GroupId, cancellationToken),
            CreatedById = _authenticatedUserContext.UserId,
            CreatedAtUtc = _clockService.UtcNow,
            ArticleReferences = GetReferenceEntities(request.Text),
            ArticleTags = [],
        };

        foreach (var t in request.Tags)
        {
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(i => i.Name == t, cancellationToken);

            if (tag is null || tag.Id == 0)
            {
                tag = new() { Name = t.ToLowerInvariant() };
                await _dbContext.Tags.AddAsync(tag, cancellationToken);
            }

            article.ArticleTags.Add(new() { Tag = tag });
        }

        _dbContext.Articles.Add(article);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var articleDto = await _articleLoader.LoadAsync(article.Id, cancellationToken);
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
        int GroupId,
        string Name,
        string? Icon,
        string Text,
        string Keywords,
        bool IsActive,
        bool IsAvailableOnMainScreen,
        List<string> Tags)
        : ICommand<ArticleDto>, ITransactionalRequest;

    internal sealed class CreateArticleRequestValidator : AbstractValidator<Command>
    {
        public CreateArticleRequestValidator()
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
