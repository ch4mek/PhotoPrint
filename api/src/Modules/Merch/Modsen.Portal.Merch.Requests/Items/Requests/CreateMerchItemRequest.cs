using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Items.Core.Composers;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Items.Core.Validators;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class CreateMerchItemRequest : ICommandHandler<CreateMerchItemRequest.Command, ItemDto>
{
    private readonly IIsItemNameUniqueBusinessRule _isItemNameUniqueBusinessRule;
    private readonly IItemSizeMinAmountValidator _itemSizeMinAmountValidator;
    private readonly IItemSizeMinAmountComposer _sizeMinAmountComposer;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IMerchDbContext _dbContext;
    private readonly IClockService _clockService;

    public CreateMerchItemRequest(IIsItemNameUniqueBusinessRule isItemNameUniqueBusinessRule,
        IItemSizeMinAmountValidator itemSizeMinAmountValidator,
        IItemSizeMinAmountComposer sizeMinAmountComposer,
        IAuthenticatedUserContext authenticatedUserContext,
        IMerchDbContext dbContext,
        IClockService clockService)
    {
        _isItemNameUniqueBusinessRule = isItemNameUniqueBusinessRule;
        _itemSizeMinAmountValidator = itemSizeMinAmountValidator;
        _sizeMinAmountComposer = sizeMinAmountComposer;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _dbContext = dbContext;
    }

    public async Task<ItemDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        var isNameUnique = await _isItemNameUniqueBusinessRule.IsCorrespondsToBusiness(
            id: null,
            name: request.Name,
            cancellationToken);

        if (!isNameUnique)
        {
            throw new InvalidDataAppException(PortalMessages.MerchItemNameExists);
        }

        var itemToCreate = new ItemEntity
        {
            Name = request.Name,
            Description = request.Description,
            IsClothing = request.IsClothing,
            Supplier = request.Supplier,
            Status = MerchItemStatus.Available,
            PhotoFileId = request.PhotoFileId,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId!.Value
        };

        var itemEntry = await _dbContext.Items.AddAsync(itemToCreate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        itemToCreate.Id = itemEntry.Entity.Id;

        var minAmounts = request.SizeMinAmounts.Select(SizeMinAmountCoreDto.Of).ToList();

        await _itemSizeMinAmountValidator.ValidateAsync(minAmounts, request.IsClothing, cancellationToken);

        var minAmountsToCreate = _sizeMinAmountComposer.Compose(itemEntry.Entity.Id,
            minAmounts);

        _dbContext.ItemSizeMinAmounts.AddRange(minAmountsToCreate);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ItemDto.Of(itemToCreate, minAmountsToCreate);
    }

    internal sealed record Command(CreateItemDto RequestDto) : ICommand<ItemDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.RequestDto).SetValidator(new CreateItemDto.Validator());
        }
    }

}
