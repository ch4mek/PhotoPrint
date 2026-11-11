using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests;

internal sealed class CreateDiscountCategoryRequest : ICommandHandler<CreateDiscountCategoryRequest.Command, DiscountCategoryCoreDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountCategoryLoader _discountCategoryLoader;

    public CreateDiscountCategoryRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountCategoryLoader discountCategoryLoader)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountCategoryLoader = discountCategoryLoader;
    }

    public async Task<DiscountCategoryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (await _knowledgeBaseDbContext.DiscountCategories.AnyAsync(x =>
        x.Name.ToLower() == request.Name.ToLower(), cancellationToken))
        {
            throw new InvalidDataAppException("Discount category with the same name already exists");
        }

        var discountCategory = new DiscountCategoryEntity
        {
            Name = request.Name,
            OrderWeight = await CalculateOrderWeight(cancellationToken),
            DiscountCategoryTags = await GetTagEntities(request.DiscountCategoryTags ?? [], cancellationToken)
        };

        _knowledgeBaseDbContext.DiscountCategories.Add(discountCategory);

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return await _discountCategoryLoader.Load(discountCategory.Id, cancellationToken);
    }

    private async Task<List<DiscountCategoryTagEntity>> GetTagEntities(IReadOnlyCollection<string> discountCategoryTags, CancellationToken cancellationToken)
    {
        var tagsEntities = new List<DiscountCategoryTagEntity>();

        discountCategoryTags = discountCategoryTags.Select(x => x.ToLower()).ToList();

        foreach (var tagName in discountCategoryTags)
        {
            var tag = await _knowledgeBaseDbContext.Tags.FirstOrDefaultAsync(i => i.Name == tagName, cancellationToken);

            if (tag == null)
            {
                tag = new()
                {
                    Name = tagName.ToLower()
                };
                await _knowledgeBaseDbContext.Tags.AddAsync(tag, cancellationToken);
            }

            tagsEntities.Add(new()
            {
                Tag = tag
            });
        }

        return tagsEntities;
    }

    private async Task<int> CalculateOrderWeight(CancellationToken cancellationToken)
    {
        var maxOrderWeight = (await _knowledgeBaseDbContext.DiscountCategories
            .Select(x => x.OrderWeight)
            .ToListAsync(cancellationToken))
            .DefaultIfEmpty(0)
            .Max();

        return maxOrderWeight + 1;
    }

    internal sealed record Command : ICommand<DiscountCategoryCoreDto>, ITransactionalRequest
    {
        public required string Name { get; set; }
        public IReadOnlyCollection<string>? DiscountCategoryTags { get; set; }
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .MinimumLength(2).MaximumLength(250);
        }
    }
}
