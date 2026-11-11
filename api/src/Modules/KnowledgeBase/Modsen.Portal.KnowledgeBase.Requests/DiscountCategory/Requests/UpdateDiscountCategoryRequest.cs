using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.BusinessRules;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests;

internal sealed class UpdateDiscountCategoryRequest : ICommandHandler<UpdateDiscountCategoryRequest.Command, DiscountCategoryCoreDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountCategoryLoader _discountCategoryLoader;
    private readonly IHasTagAnyDiscountsInCategory _hasTagAnyDiscountsInCategory;

    public UpdateDiscountCategoryRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountCategoryLoader discountCategoryLoader,
        IHasTagAnyDiscountsInCategory hasTagAnyDiscountsInCategory)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountCategoryLoader = discountCategoryLoader;
        _hasTagAnyDiscountsInCategory = hasTagAnyDiscountsInCategory;
    }

    public async Task<DiscountCategoryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _knowledgeBaseDbContext.DiscountCategories
            .Include(x => x.DiscountCategoryTags)
            .ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"Discount category by id {request.Id} is not found");

        if (entity.Name.ToLower() != request.Name.ToLower()
            && await _knowledgeBaseDbContext.DiscountCategories.AnyAsync(x =>
            x.Name.ToLower() == request.Name.ToLower(), cancellationToken))
        {
            throw new InvalidDataAppException("Discount category with the same name already exists");
        }

        await UpdateTags(entity.DiscountCategoryTags, request.Id, request.DiscountCategoryTags ?? [], cancellationToken);

        entity.Id = request.Id;
        entity.Name = request.Name;

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return await _discountCategoryLoader.Load(entity.Id, cancellationToken);
    }

    private async Task UpdateTags(
    List<DiscountCategoryTagEntity> discountCategoryTagEntities,
    int discountCategoryId,
    IReadOnlyCollection<string> discountCategoryTagNames,
    CancellationToken cancellationToken)
    {
        discountCategoryTagNames = discountCategoryTagNames.Select(x => x.ToLower()).ToList();

        var existingTagNames = discountCategoryTagEntities.Select(x => x.Tag!.Name).ToList();

        var namesToDelete = existingTagNames.Except(discountCategoryTagNames).ToList();

        if (namesToDelete.Count != 0)
        {
            if (await _hasTagAnyDiscountsInCategory
                .IsCorrespondsToBusiness(namesToDelete, discountCategoryId, cancellationToken))
            {
                throw new EntityConflictException("You can't delete tag, while there are discounts with these tag");
            }

            var entitiesToDelete = discountCategoryTagEntities
                .Where(e => namesToDelete.Contains(e.Tag!.Name));

            _knowledgeBaseDbContext.DiscountCategoryTags.RemoveRange(entitiesToDelete);
        }

        var namesToAdd = discountCategoryTagNames.Except(existingTagNames).ToList();

        if (namesToAdd.Count != 0)
        {
            var existingTagsToAdd = await _knowledgeBaseDbContext.Tags
                .Where(x => namesToAdd.Contains(x.Name))
                .ToListAsync(cancellationToken);

            var notExistingTagNames = namesToAdd
                .Except(existingTagsToAdd
                    .Select(x => x.Name)).ToList();

            if (notExistingTagNames.Count != 0)
            {
                var newTags = notExistingTagNames.Select(x => new TagEntity { Name = x });

                existingTagsToAdd.AddRange(newTags);
            }

            if (existingTagsToAdd.Count != 0)
            {
                var entitiesToAdd = existingTagsToAdd.Select(x => new DiscountCategoryTagEntity
                {
                    DiscountCategoryId = discountCategoryId,
                    Tag = x
                });

                _knowledgeBaseDbContext.DiscountCategoryTags.AddRange(entitiesToAdd);
            }
        }
    }

    internal sealed record Command : ICommand<DiscountCategoryCoreDto>, ITransactionalRequest
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public IReadOnlyCollection<string>? DiscountCategoryTags { get; set; } = [];
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
