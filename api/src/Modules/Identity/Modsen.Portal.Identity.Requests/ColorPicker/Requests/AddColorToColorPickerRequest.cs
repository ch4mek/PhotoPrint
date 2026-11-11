using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ColorPicker.Requests;

internal sealed class AddColorToColorPickerRequest : ICommandHandler<AddColorToColorPickerRequest.Command, string>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;

    public AddColorToColorPickerRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
    }

    public async Task<string> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingColors = await _identityDbContext.ColorPickers
            .Where(x => x.UserId == _authenticatedUserContext.GetRequiredUserId())
            .Select(x => x.Color)
            .ToListAsync(cancellationToken);

        if (existingColors.Count == 14)
        {
            throw new InvalidDataAppException("You can't add more than 14 colors");
        }

        if (existingColors.Any(x => x == request.Color))
        {
            throw new InvalidDataAppException("Color already exists");
        }

        var newColor = new DataAccess.Npgsql.DatabaseEntities.ColorPicker
        {
            Color = request.Color,
            UserId = _authenticatedUserContext.GetRequiredUserId(),
        };

        _identityDbContext.ColorPickers.Add(newColor);

        await _identityDbContext.SaveChanges(cancellationToken);

        return newColor.Color;
    }

    internal sealed record Command(string Color) : ICommand<string>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Color)
                .Length(7);
        }
    }
}
