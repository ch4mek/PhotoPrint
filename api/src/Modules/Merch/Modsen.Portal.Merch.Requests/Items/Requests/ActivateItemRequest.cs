using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class ActivateItemRequest : ICommandHandler<ActivateItemRequest.Command, ItemStatusDto>
{
    private readonly IGetMerchItemByIdDbQuery _getItemDbQuery;
    private readonly IAuthenticatedUserContext _userContext;
    private readonly IMerchDbContext _merchDbContext;

    public ActivateItemRequest(IGetMerchItemByIdDbQuery getItemDbQuery,
        IMerchDbContext merchDbContext,
        IAuthenticatedUserContext userContext)
    {
        _getItemDbQuery = getItemDbQuery;
        _merchDbContext = merchDbContext;
        _userContext = userContext;
    }

    public async Task<ItemStatusDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var item = await _getItemDbQuery.ExecuteAsync(request.ItemId, cancellationToken)
            ?? throw new InvalidDataAppException(PortalMessages.MerchItemNotFound);

        if (item.Status == MerchItemStatus.Available)
        {
            return ItemStatusDto.Of(item);
        }

        item.Status = MerchItemStatus.Available;
        item.ArchivedAtUtc = null;
        item.ArchivedById = null;
        item.UpdatedAtUtc = DateTime.UtcNow;
        item.UpdatedById = _userContext.UserId;

        await _merchDbContext.SaveChangesAsync(cancellationToken);

        return ItemStatusDto.Of(item);
    }

    internal sealed record Command(int ItemId) : ICommand<ItemStatusDto>, ITransactionalRequest;
}
