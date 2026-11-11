using FluentValidation;
using Modsen.Portal.Certificate.UseCases.CertificatePortalSettings;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.ModsenApi.Contract;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Certificate.UseCases.Requests;

internal sealed class CreateCertificateCommandUseCase : ICommandHandler<CreateCertificateCommandUseCase.Command, CertificateDto>
{
    private readonly IIdentityContract _identityContract;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IModsenApiCertificatesClient _modsenApiCertificatesClient;
    private readonly ICertificatePortalSettingsAccessor _certificatePortalSettingsAccessor;

    public CreateCertificateCommandUseCase(IIdentityContract identityContract,
        ITeamBoardContract teamBoardContract,
        IModsenApiCertificatesClient modsenApiCertificatesClient,
        ICertificatePortalSettingsAccessor certificatePortalSettingsAccessor)
    {
        _identityContract = identityContract;
        _teamBoardContract = teamBoardContract;
        _modsenApiCertificatesClient = modsenApiCertificatesClient;
        _certificatePortalSettingsAccessor = certificatePortalSettingsAccessor;
    }

    public async Task<CertificateDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var createModel = CreateCertificateDto.ToModel(request.Model);

        var settings = await _certificatePortalSettingsAccessor.GetSettings(cancellationToken);

        if (!int.TryParse(createModel.RecipientId, out var userId))
        {
            throw new InvalidDataAppException("User id must be a number");
        }

        var userRoles = await _identityContract.UserRoles(userId, cancellationToken);

        if (!userRoles.IsEmployee())
        {
            throw new ActionNotAllowedException("It not possible to add certificate to User with not Employee role ");
        }

        await EnsureUserCanCreateAsync(userId, cancellationToken);

        ArgumentException.ThrowIfNullOrEmpty(createModel.Technology);

        var apiValue = await _identityContract.GetDepartmentModsenApiValue(createModel.Technology, cancellationToken)
            ?? throw new EntityNotFoundException($"Department with id = {createModel.Technology} not found!");

        createModel.Technology = apiValue;

        ArgumentException.ThrowIfNullOrEmpty(settings.MonthDayYearFormat);

        var certificate = await _modsenApiCertificatesClient
            .CreateUserCertificate(createModel, settings.MonthDayYearFormat, cancellationToken);

        return CertificateDto.Of(certificate);
    }

    private async Task EnsureUserCanCreateAsync(int userId, CancellationToken cancellationToken)
    {
        var availabilityMap = await _teamBoardContract.CanUsersBeRetrievedByUser([userId], AccessOptions.Manager, cancellationToken);

        var isContains = availabilityMap.TryGetValue(userId, out var isAvailable);
        if (!isContains || !isAvailable)
        {
            throw new ActionNotAllowedException($"You cannot create a certificate for user {userId}");
        }
    }

    internal sealed record Command(CreateCertificateDto Model)
        : ICommand<CertificateDto>, ICertificateRequest, ITransactionalRequest;


    internal sealed class CreateCertificateCommandValidator : AbstractValidator<Command>
    {
        public CreateCertificateCommandValidator()
        {
            RuleFor(x => x.Model.Recipient)
                .LongAlphabeticWithHyphensSpaces();

            RuleFor(x => x.Model.Stack)
                .Must((model, _) =>
                    model.Model is { Technology: "Js", Stack: not null } ||
                    model.Model.Technology != "Js" && !model.Model.Stack.HasValue)
                .WithMessage("Specify Stack for JS Department or remove it for other Departments")
                .WithName(nameof(Command.Model.Stack));
        }
    }
}
