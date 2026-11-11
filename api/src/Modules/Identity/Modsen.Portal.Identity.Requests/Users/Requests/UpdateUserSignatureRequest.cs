using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateUserSignatureRequest : ICommandHandler<UpdateUserSignatureRequest.Command, UserSignatureDto>{
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IIdentityDbContext _identityDbContext;

    public UpdateUserSignatureRequest(
        IAuthenticatedUserContext authenticatedUser,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUser = authenticatedUser;
        _identityDbContext = identityDbContext;
    }

    public async Task<UserSignatureDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _identityDbContext.Users.FirstOrDefaultAsync(u => u.Id == _authenticatedUser.GetRequiredUserId(), cancellationToken)
            ?? throw new EntityNotFoundException($"User with {_authenticatedUser.GetRequiredUserId()} ID was not found");

        user.SignatureFullName = request.FullName.Trim();
        user.SignaturePosition = request.Position.Trim();
        user.SignaturePhone = request.Phone;

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

    internal sealed record Command(
        string FullName,
        string Position,
        string? Phone) : ICommand<UserSignatureDto>, ITransactionalRequest, IIdentityRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FullName)
                .NotNull()
                .LongAlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule()
                .WithName(nameof(Command.FullName));

            RuleFor(x => x.Position)
                .NotNull()
                .AlphabeticWithPunctuationChars()
                .SpacesOrPunctuationSymbolsRule()
                .WithName(nameof(Command.Position));

            RuleFor(x => x.Phone)
                .NotNull()
                .PhoneNumberType();
        }
    }
}
