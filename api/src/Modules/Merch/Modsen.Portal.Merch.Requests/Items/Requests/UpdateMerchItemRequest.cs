using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.BusinessRules;
using Modsen.Portal.Merch.Requests.Items.Core.Composers;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Core.Validators;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class UpdateMerchItemRequest : ICommandHandler<UpdateMerchItemRequest.Command, ItemDto>
{
    private readonly IIsItemNameUniqueBusinessRule _isNameUniqueBusinessRule;
    private readonly IItemSizeMinAmountValidator _itemSizeMinAmountValidator;
    private readonly IItemSizeMinAmountComposer _sizeMinAmountComposer;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetMerchItemByIdDbQuery _getMerchItemById;
    private readonly IMerchDbContext _dbContext;
    private readonly IClockService _clockService;

    public UpdateMerchItemRequest(IIsItemNameUniqueBusinessRule isNameUniqueBusinessRule,
        IItemSizeMinAmountValidator itemSizeMinAmountValidator,
        IItemSizeMinAmountComposer sizeMinAmountComposer,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetMerchItemByIdDbQuery getMerchItemById,
        IMerchDbContext dbContext,
        IClockService clockService)
    {
        _itemSizeMinAmountValidator = itemSizeMinAmountValidator;
        _authenticatedUserContext = authenticatedUserContext;
        _isNameUniqueBusinessRule = isNameUniqueBusinessRule;
        _sizeMinAmountComposer = sizeMinAmountComposer;
        _getMerchItemById = getMerchItemById;
        _clockService = clockService;
        _dbContext = dbContext;
    }

    public async Task<ItemDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;
        var merchItem = await _getMerchItemById.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.MerchItemNotFound);

        if (merchItem.Status == MerchItemStatus.Archived)
        {
            throw new InvalidDataAppException(PortalMessages.MerchItemIsNotAvailable);
        }

        if (merchItem.IsClothing != request.IsClothing)
        {
            throw new InvalidDataAppException();
        }

        var isNameUnique = await _isNameUniqueBusinessRule.IsCorrespondsToBusiness(
            id: request.Id,
            name: request.Name,
            cancellationToken);

        if (!isNameUnique)
        {
            throw new InvalidDataAppException(PortalMessages.MerchItemNameExists);
        }

        merchItem.Name = request.Name;
        merchItem.Supplier = request.Supplier;
        merchItem.Description = request.Description;
        merchItem.PhotoFileId = request.PhotoFileId;
        merchItem.UpdatedAtUtc = _clockService.UtcNow;
        merchItem.UpdatedById = _authenticatedUserContext.UserId!.Value;

        await _dbContext.ItemSizeMinAmounts
            .Where(size => size.ItemId == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        var minAmountCoreDtos = request.SizeMinAmounts.Select(SizeMinAmountCoreDto.Of).ToList();

        await _itemSizeMinAmountValidator.ValidateAsync(minAmountCoreDtos, request.IsClothing, cancellationToken);

        var minAmounts = _sizeMinAmountComposer.Compose(merchItem.Id,
            minAmountCoreDtos);

        _dbContext.ItemSizeMinAmounts.AddRange(minAmounts);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ItemDto.Of(merchItem, minAmounts);
    }

    internal sealed record Command(UpdateItemDto RequestDto) : ICommand<ItemDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.RequestDto).SetValidator(new UpdateItemDto.Validator());
        }
    }
}
