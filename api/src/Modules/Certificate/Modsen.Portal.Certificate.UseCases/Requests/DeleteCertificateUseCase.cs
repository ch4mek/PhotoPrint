using System.Globalization;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.ModsenApi.Contract;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Certificate.UseCases.Requests;

internal sealed class DeleteCertificateUseCase : ICommandHandler<DeleteCertificateUseCase.Command, CertificateDto>
{
    private readonly IModsenApiCertificatesClient _apiCertificatesClient;
    private readonly ITeamBoardContract _teamBoardContract; 


    public DeleteCertificateUseCase(IModsenApiCertificatesClient apiCertificatesClient,
        ITeamBoardContract teamBoardContract)
    {
        _apiCertificatesClient = apiCertificatesClient;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<CertificateDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!int.TryParse(request.UserId, CultureInfo.InvariantCulture, out var userId))
        {
            throw new InvalidDataAppException("Provided User's Id is not a number");
        }

        await EnsureUserCanDeleteAsync(userId, cancellationToken);

        var userCertificates = await _apiCertificatesClient.GetUserCertificates(request.UserId, cancellationToken);
        if (userCertificates.All(x => x.CertificateId != request.CertificateId))
        {
            throw new ActionNotAllowedException("The provided user does not have provided certificate");
        }

        var deletedCertificate = await _apiCertificatesClient
            .DeleteUserCertificate(request.CertificateId, cancellationToken);

        return CertificateDto.Of(deletedCertificate);
    }

    private async Task EnsureUserCanDeleteAsync(int userId, CancellationToken cancellationToken)
    {
        var availabilityMap = await _teamBoardContract.CanUsersBeRetrievedByUser([userId], AccessOptions.Manager, cancellationToken);

        var isContains = availabilityMap.TryGetValue(userId, out var isAvailable);
        if (!isContains || !isAvailable)
        {
            throw new ActionNotAllowedException($"You cannot remove a certificate for user {userId}");
        }
    }

    internal sealed record Command(string CertificateId, string UserId) : ICommand<CertificateDto>, ITransactionalRequest;
}
