using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.BusinessRules;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class CreateDiscountRequest : ICommandHandler<CreateDiscountRequest.Command, DiscountCoreDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountLoader _discountLoader;
    private readonly IHasDiscountCategoryAllDiscountTagsBusinessRule _hasDiscountCategoryAllDiscountTagsBusinessRule;
    private readonly IImageFileIdValidator _imageFileIdValidator;

    public CreateDiscountRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountLoader discountLoader,
        IHasDiscountCategoryAllDiscountTagsBusinessRule hasDiscountCategoryAllDiscountTagsBusinessRule,
        IImageFileIdValidator imageFileIdValidator)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountLoader = discountLoader;
        _hasDiscountCategoryAllDiscountTagsBusinessRule = hasDiscountCategoryAllDiscountTagsBusinessRule;
        _imageFileIdValidator = imageFileIdValidator;
    }

    public async Task<DiscountCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var discountCategory = await _knowledgeBaseDbContext.DiscountCategories.FirstOrDefaultAsync(x => x.Id == request.DiscountCategoryId, cancellationToken)
            ?? throw new EntityNotFoundException($"Discount category with Id {request.DiscountCategoryId} doesn't exist.");

        if (await _knowledgeBaseDbContext.Discounts.AnyAsync(x => x.Name.ToLower() == request.Name.ToLower(), cancellationToken))
        {
            throw new InvalidDataAppException("Discount with the same name already exists");
        }

        await _imageFileIdValidator.Validate(request.ImageFileId, cancellationToken);

        var discountEntity = new DiscountEntity
        {
            DiscountCategory = discountCategory,
            DiscountCategoryId = discountCategory.Id,
            ImageUrl = request.ImageUrl,
            ImageFileId = request.ImageFileId,
            IsActive = request.IsActive,
            Name = request.Name,
            OrderWeight = await CalculateOrderWeight(cancellationToken),
            Text = request.Text,
            DiscountPercent = request.DiscountPercent,
            DiscountPercentTo = request.DiscountPercentTo,
            CompanyType = request.CompanyType,
            Website = request.Website,
            PhoneNumber = request.PhoneNumber,
            Location = request.Location,
            DiscountCities = await GetCitiesEntities(request.DiscountCityIds, cancellationToken),
            DiscountTags = await GetTagEntities(request.DiscountTagIds, request.DiscountCategoryId, cancellationToken),
            DiscountReferences = GetReferenceEntities(request.DiscountSocialMediaUrls)
        };

        _knowledgeBaseDbContext.Discounts.Add(discountEntity);
        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return (await _discountLoader.Load(discountEntity.Id, cancellationToken))!;
    }

    private async Task<List<DiscountTagEntity>> GetTagEntities(IReadOnlyCollection<int>? discountTagIds, int discountCategoryId, CancellationToken cancellationToken)
    {
        if (discountTagIds != null
            && discountTagIds.Count != 0
            && !await _hasDiscountCategoryAllDiscountTagsBusinessRule
                .IsCorrespondsToBusiness(discountTagIds, discountCategoryId, cancellationToken))
        {

            throw new InvalidDataAppException("You can add tag only from chosen discount category");
        }

        return discountTagIds?.Select(x => new DiscountTagEntity
        {
            TagId = x
        }).ToList()
        ?? [];
    }

    private async Task<List<DiscountCityEntity>> GetCitiesEntities(IReadOnlyCollection<int> discountCityIds, CancellationToken cancellationToken)
    {
        var cityIds = await _knowledgeBaseDbContext.Cities.Select(x => x.Id).ToListAsync(cancellationToken);

        if (!discountCityIds.All(cityIds.Contains))
        {
            throw new InvalidDataAppException("You can add only existing cities");
        }

        return discountCityIds.Select(x => new DiscountCityEntity
        {
            CityId = x
        }).ToList();
    }

    private static List<DiscountReferenceEntity> GetReferenceEntities(IReadOnlyCollection<string>? discountSocialMediaUrls)
    {
        return discountSocialMediaUrls?.Select(x => new DiscountReferenceEntity
        {
            SocialMediaUrl = x
        }).ToList()
        ?? [];
    }

    private async Task<int> CalculateOrderWeight(CancellationToken cancellationToken)
    {
        var maxOrderWeight = (await _knowledgeBaseDbContext.Discounts
            .Select(x => x.OrderWeight)
            .ToListAsync(cancellationToken))
            .DefaultIfEmpty(0)
            .Max();

        return maxOrderWeight + 1;
    }

    internal sealed record Command : ICommand<DiscountCoreDto>, ITransactionalRequest
    {
        public int DiscountCategoryId { get; set; }
        public required string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageFileId { get; set; }
        public int DiscountPercent { get; set; }
        public int? DiscountPercentTo { get; set; }
        public string? CompanyType { get; set; }
        public string? Website { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public required string Text { get; set; }
        public bool IsActive { get; set; }

        public IReadOnlyCollection<int>? DiscountTagIds { get; set; } = [];
        public IReadOnlyCollection<int> DiscountCityIds { get; set; } = [];
        public IReadOnlyCollection<string>? DiscountSocialMediaUrls { get; set; } = [];
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .MinimumLength(2).MaximumLength(250);

            RuleFor(x => x.ImageFileId).MaximumLength(128);

            RuleFor(x => x.DiscountPercent)
                .NotNull()
                .GreaterThan(0)
                .LessThan(100);

            RuleFor(x => x.DiscountPercentTo)
                .GreaterThan(0)
                .LessThan(100)
                .GreaterThan(x => x.DiscountPercent);

            RuleFor(x => x.CompanyType)
                .MaximumLength(250);

            RuleFor(x => x.PhoneNumber)
                .MinimumLength(2)
                .MaximumLength(250)
                .Matches(@"\d");

            RuleFor(x => x.Location)
                .MaximumLength(250);

            RuleFor(x => x.DiscountCityIds)
                .NotNull().NotEmpty();
        }
    }
}

