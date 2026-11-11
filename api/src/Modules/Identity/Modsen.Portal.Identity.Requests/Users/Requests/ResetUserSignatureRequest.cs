using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class ResetUserSignatureRequest : ICommandHandler<ResetUserSignatureRequest.Command, UserSignatureDto>{
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IIdentityDbContext _identityDbContext;

    public ResetUserSignatureRequest(
        IAuthenticatedUserContext authenticatedUser,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUser = authenticatedUser;
        _identityDbContext = identityDbContext;
    }

    public async Task<UserSignatureDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _identityDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == _authenticatedUser.GetRequiredUserId(), cancellationToken)
            ?? throw new EntityNotFoundException($"User with {_authenticatedUser.GetRequiredUserId()} ID was not found");

        user.SignatureFullName = null;
        user.SignaturePosition = null;
        user.SignaturePhone = null;

        await _identityDbContext.SaveChanges(cancellationToken);

        var updatedUser = await _identityDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == _authenticatedUser.GetRequiredUserId(), cancellationToken);

        return new UserSignatureDto
        {
            FullName = !(updatedUser?.SignatureFullName).IsNullOrWhiteSpace()
                    ? updatedUser.SignatureFullName
                    : ToFullName.Reverse(updatedUser?.FirstName, updatedUser?.LastName),
            Position = updatedUser?.SignaturePosition,
            Phone = updatedUser?.SignaturePhone,
            Email = updatedUser?.Email
        };
    }

    internal sealed record Command : ICommand<UserSignatureDto>, ITransactionalRequest, IIdentityRequest;
}
