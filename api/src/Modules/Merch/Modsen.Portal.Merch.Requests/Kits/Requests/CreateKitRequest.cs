using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Validators;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests;

internal sealed class CreateKitRequest : ICommandHandler<CreateKitRequest.Command, KitDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICreateOrUpdateKitValidator _createKitValidator;
    private readonly IItemLookupLoader _itemLookupLoader;
    private readonly IClockService _clockService;
    private readonly IMerchDbContext _dbContext;

    public CreateKitRequest(IAuthenticatedUserContext authenticatedUserContext,
        ICreateOrUpdateKitValidator createKitValidator,
        IItemLookupLoader itemLookupLoader,
        IClockService clockService,
        IMerchDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _createKitValidator = createKitValidator;
        _itemLookupLoader = itemLookupLoader;
        _clockService = clockService;
        _dbContext = dbContext;
    }

    public async Task<KitDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var requestDto = request.RequestDto;

        await _createKitValidator.ValidateAsync(CreateOrUpdateKitValidatorCoreDto.Of(requestDto), cancellationToken);

        var compounds = await _itemLookupLoader.LoadAsync(
            requestDto.Compounds.Select(compound => compound.ItemId).ToArray(),
            cancellationToken);

        var kitToCreate = new KitEntity
        {
            Name = requestDto.Name,
            Description = requestDto.Description,
            PhotoFileId = requestDto.PhotoFileId,
            CreatedById = _authenticatedUserContext.GetRequiredUserId(),
            CreatedAtUtc = _clockService.UtcNow,
            KitMinAmounts = requestDto.MinAmounts
                .Select(amount => amount.ToEntity())
                .ToList(),
            KitItems = requestDto.Compounds
                .Select(compound => compound.ToEntity())
                .ToList()
        };

        _dbContext.Kits.Add(kitToCreate);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return KitDto.Of(kitToCreate, compounds);
    }

    internal sealed record Command(CreateKitDto RequestDto) : ICommand<KitDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.RequestDto).SetValidator(new CreateKitDto.Validator());
        }
    }
}
