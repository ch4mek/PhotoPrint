using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ColorPicker.Requests;

internal sealed class GetUserColorsFromColorPickerRequest : IQueryHandler<GetUserColorsFromColorPickerRequest.Query, List<string>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;

    public GetUserColorsFromColorPickerRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _identityDbContext.ColorPickers
            .Where(x => x.UserId == _authenticatedUserContext.GetRequiredUserId())
            .Select(x => x.Color)
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<string>>;
}
