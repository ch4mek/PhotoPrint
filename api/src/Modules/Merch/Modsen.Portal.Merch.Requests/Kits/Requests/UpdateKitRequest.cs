using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Validators;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests;

internal sealed class UpdateKitRequest : ICommandHandler<UpdateKitRequest.Command, KitDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICreateOrUpdateKitValidator _updateKitValidator;
    private readonly IItemLookupLoader _itemLookupLoader;
    private readonly IClockService _clockService;
    private readonly IMerchDbContext _dbContext;

    public UpdateKitRequest(IAuthenticatedUserContext authenticatedUserContext,
        ICreateOrUpdateKitValidator updateKitValidator,
        IItemLookupLoader itemLookupLoader,
        IClockService clockService,
        IMerchDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _updateKitValidator = updateKitValidator;
        _itemLookupLoader = itemLookupLoader;
        _clockService = clockService;
        _dbContext = dbContext;
    }

    public async Task<KitDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var requestDto = request.RequestDto;

        await _updateKitValidator.ValidateAsync(CreateOrUpdateKitValidatorCoreDto.Of(requestDto), cancellationToken);

        var kit = await _dbContext.Kits.FirstOrDefaultAsync(kit => kit.Id == requestDto.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.KitNotFound);

        if(kit.Status == MerchItemStatus.Archived)
        {
            throw new InvalidDataAppException(PortalMessages.KitIsNotAvailable);
        }

        var compounds = await _itemLookupLoader.LoadAsync(
            requestDto.Compounds.Select(compound => compound.ItemId).ToArray(),
            cancellationToken);

        var minAmountsToDelete = await _dbContext.KitMinAmounts.Where(amount => amount.KitId == kit.Id)
            .ToListAsync(cancellationToken);
        _dbContext.KitMinAmounts.RemoveRange(minAmountsToDelete);

        var kitItemsToRemove = await _dbContext.KitItems.Where(item => item.KitId == kit.Id)
            .ToListAsync(cancellationToken);
        _dbContext.KitItems.RemoveRange(kitItemsToRemove);

        kit.Name = requestDto.Name;
        kit.Description = requestDto.Description;
        kit.PhotoFileId = requestDto.PhotoFileId;

        kit.KitMinAmounts = requestDto.MinAmounts
            .Select(amount => amount.ToEntity())
            .ToList();
        kit.KitItems = requestDto.Compounds
            .Select(compound => compound.ToEntity())
            .ToList();

        kit.UpdatedAtUtc = _clockService.UtcNow;
        kit.UpdatedById = _authenticatedUserContext.GetRequiredUserId();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return KitDto.Of(kit, compounds);
    }

    internal sealed record Command(UpdateKitDto RequestDto) : ICommand<KitDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.RequestDto)
                .SetValidator(new UpdateKitDto.Validator());
        }
    }
}
