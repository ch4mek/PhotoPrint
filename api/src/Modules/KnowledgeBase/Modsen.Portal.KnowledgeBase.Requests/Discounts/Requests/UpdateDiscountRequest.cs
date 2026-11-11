using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.BusinessRules;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class UpdateDiscountRequest : ICommandHandler<UpdateDiscountRequest.Command, DiscountCoreDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountLoader _discountLoader;
    private readonly IHasDiscountCategoryAllDiscountTagsBusinessRule _hasDiscountCategoryAllDiscountTagsBusinessRule;
    private readonly ICommunicationBus _communicationBus;
    private readonly IImageFileIdValidator _imageFileIdValidator;

    public UpdateDiscountRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountLoader discountLoader,
        IHasDiscountCategoryAllDiscountTagsBusinessRule hasDiscountCategoryAllDiscountTagsBusinessRule,
        ICommunicationBus communicationBus,
        IImageFileIdValidator imageFileIdValidator)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountLoader = discountLoader;
        _hasDiscountCategoryAllDiscountTagsBusinessRule = hasDiscountCategoryAllDiscountTagsBusinessRule;
        _communicationBus = communicationBus;
        _imageFileIdValidator = imageFileIdValidator;
    }

    public async Task<DiscountCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await GetDiscountEntity(request.Id, cancellationToken);

        if (request.DiscountCategoryId != entity.DiscountCategoryId &&
            !await _knowledgeBaseDbContext.DiscountCategories
                .AnyAsync(x => x.Id == request.DiscountCategoryId, cancellationToken))
        {
            throw new EntityConflictException("You can't add a category that doesn't exist");
        }

        if (entity.Name.ToLower(CultureInfo.InvariantCulture) != request.Name.ToLower(CultureInfo.InvariantCulture)
            && await _knowledgeBaseDbContext.Discounts.AnyAsync(x =>
            x.Name.ToLower() == request.Name.ToLower(), cancellationToken))
        {
            throw new InvalidDataAppException("Discount with the same name already exists");
        }

        await _imageFileIdValidator.Validate(request.ImageFileId, cancellationToken);

        await UpdateTags(entity.DiscountTags, request.Id, request.DiscountCategoryId, request.DiscountTagIds ?? [], cancellationToken);

        await UpdateCities(entity.DiscountCities, request.Id, request.DiscountCityIds, cancellationToken);

        _knowledgeBaseDbContext.DiscountsReferences.RemoveRange(entity.DiscountReferences);
        UpdateReferences(entity.DiscountReferences, request.DiscountSocialMediaUrls);

        if ((request.ImageFileId is null && entity.ImageFileId is not null)
            || (request.ImageFileId is not null && entity.ImageFileId is not null && request.ImageFileId != entity.ImageFileId))
        {
            var deleteFileNotification = new DeleteFileEvent(entity.ImageFileId, Guid.CreateVersion7().ToString());

            await _communicationBus.PublishAsync(deleteFileNotification, cancellationToken);
        }

        entity.DiscountCategoryId = request.DiscountCategoryId;
        entity.Name = request.Name;
        entity.ImageUrl = request.ImageUrl;
        entity.ImageFileId = request.ImageFileId;
        entity.DiscountPercent = request.DiscountPercent;
        entity.DiscountPercentTo = request.DiscountPercentTo;
        entity.CompanyType = request.CompanyType;
        entity.Website = request.Website;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Location = request.Location;
        entity.Text = request.Text;
        entity.IsActive = request.IsActive;

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return (await _discountLoader.Load(entity.Id, cancellationToken))!;
    }

    private async Task<DiscountEntity> GetDiscountEntity(int discountId, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.Discounts
                .Include(x => x.DiscountTags)
                .ThenInclude(bt => bt.Tag)
                .Include(x => x.DiscountCities)
                .ThenInclude(bc => bc.City)
                .Include(x => x.DiscountReferences)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == discountId, cancellationToken)
            ?? throw new EntityNotFoundException($"Discount by id {discountId} is not found");
    }

    private async Task UpdateTags(
        IReadOnlyCollection<DiscountTagEntity> discountTagEntities,
        int discountId,
        int discountCategoryId,
        IReadOnlyCollection<int>? discountTagIds,
        CancellationToken cancellationToken)
    {
        if (discountTagIds != null
            && discountTagIds.Count != 0
            && !await _hasDiscountCategoryAllDiscountTagsBusinessRule
                .IsCorrespondsToBusiness(discountTagIds, discountCategoryId, cancellationToken))
        {

            throw new InvalidDataAppException("You can add tag only from chosen discount category");
        }

        var existingTagIds = discountTagEntities.Select(x => x.TagId).ToList();

        var idsToDelete = existingTagIds.Except(discountTagIds ?? []).ToList();

        if (idsToDelete.Count != 0)
        {
            var entitiesToDelete = discountTagEntities
                .Where(e => idsToDelete.Contains(e.TagId));

            _knowledgeBaseDbContext.DiscountTags.RemoveRange(entitiesToDelete);
        }

        var idsToAdd = (discountTagIds ?? []).Except(existingTagIds).ToList();

        if (idsToAdd.Count != 0)
        {
            var entitiesToAdd = idsToAdd.Select(x => new DiscountTagEntity
            {
                DiscountId = discountId,
                TagId = x
            });

            _knowledgeBaseDbContext.DiscountTags.AddRange(entitiesToAdd);
        }
    }

    private async Task UpdateCities(
        IReadOnlyCollection<DiscountCityEntity> discountCityEntities,
        int discountId,
        IReadOnlyCollection<int> discountCityIds,
        CancellationToken cancellationToken)
    {
        var cityIds = await _knowledgeBaseDbContext.Cities.Select(x => x.Id).ToListAsync(cancellationToken);

        if (!discountCityIds.All(cityIds.Contains))
        {
            throw new InvalidDataAppException("You can add only exsisting cities");
        }

        var existingCitiesIds = discountCityEntities.Select(x => x.CityId).ToList();

        var idsToDelete = existingCitiesIds.Except(discountCityIds).ToList();

        if (idsToDelete.Count != 0)
        {
            var entitiesToDelete = discountCityEntities
                .Where(e => idsToDelete.Contains(e.CityId));

            _knowledgeBaseDbContext.DiscountCities.RemoveRange(entitiesToDelete);
        }

        var idsToAdd = discountCityIds.Except(existingCitiesIds).ToList();

        if (idsToAdd.Count != 0)
        {
            var entitiesToAdd = idsToAdd.Select(x => new DiscountCityEntity
            {
                DiscountId = discountId,
                CityId = x
            });

            _knowledgeBaseDbContext.DiscountCities.AddRange(entitiesToAdd);
        }
    }

    private static void UpdateReferences(
        List<DiscountReferenceEntity> discountReferenceEntities,
        IReadOnlyCollection<string>? discountSocialMediaUrls)
    {
        discountReferenceEntities.Clear();

        if (discountSocialMediaUrls is { Count: > 0 })
        {
            var newDiscountReferences = discountSocialMediaUrls.Select(x => new DiscountReferenceEntity
            {
                SocialMediaUrl = x
            }).ToList();

            discountReferenceEntities.AddRange(newDiscountReferences);
        }
    }

    internal sealed record Command : ICommand<DiscountCoreDto>, ITransactionalRequest
    {
        public int Id { get; set; }
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
