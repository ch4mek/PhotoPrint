using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ColorPicker.Requests;

internal sealed class DeleteColorFromColorPickerRequest : ICommandHandler<DeleteColorFromColorPickerRequest.Command, bool>
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public DeleteColorFromColorPickerRequest(
        IIdentityDbContext identityDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _identityDbContext = identityDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
    {
        var color = await _identityDbContext.ColorPickers
            .Where(x => x.Color == request.Color)
            .Where(x => x.UserId == _authenticatedUserContext.GetRequiredUserId())
            .FirstOrDefaultAsync(cancellationToken);

        if (color == null)
        {
            throw new EntityNotFoundException("User hasn't this color");
        }

        _identityDbContext.ColorPickers.Remove(color);
        await _identityDbContext.SaveChanges(cancellationToken);

        return true;
    }

    internal sealed record Command(string Color) : ICommand<bool>, ITransactionalRequest;
}
