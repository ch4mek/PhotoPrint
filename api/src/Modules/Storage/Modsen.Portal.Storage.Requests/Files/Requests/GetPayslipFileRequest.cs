using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.CoreDto;
using Modsen.Portal.Storage.Requests.Files.Core.Encryptor;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Storage.Requests.Files.Requests;

internal sealed class GetPayslipFileRequest : IQueryHandler<GetPayslipFileRequest.Query, FileResponseDto>
{
    private readonly ICloudStorageManager _cloudStorageManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IStorageDbContext _dbContext;
    private readonly IClockService _clockService;
    private readonly IPayslipEncryptor _payslipEncryptor;
    private readonly ITeamBoardContract _teamBoardContract;

    public GetPayslipFileRequest(
        ICloudStorageManager cloudStorageManager,
        IHttpClientFactory httpClientFactory,
        IStorageDbContext dbContext,
        IClockService clockService,
        IPayslipEncryptor payslipEncryptor,
        ITeamBoardContract teamBoardContract)
    {
        _cloudStorageManager = cloudStorageManager;
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _clockService = clockService;
        _payslipEncryptor = payslipEncryptor;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<FileResponseDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var file = await _dbContext.ApplicationFiles.AsNoTracking()
            .Where(file => file.FileId == request.FileId &&
                (!file.AvailableTill.HasValue || file.AvailableTill.Value > _clockService.UtcNow))
            .Select(file => new FileCoreDto
            {
                FilePath = file.FilePath,
                VirtualDirectory = file.VirtualDirectory,
                FileId = file.FileId,
                ContentType = file.ContentType,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null)
        {
            throw new EntityNotFoundException(PortalMessages.FileIsNotFound);
        }

        var isUserHaveAccessToFile = await _teamBoardContract.IsUserHaveAccessToPayslipFile(file.FileId, cancellationToken);

        if (!isUserHaveAccessToFile)
        {
            throw new ActionNotAllowedException("You cannot access this file");
        }

        return await GetFile(file, cancellationToken);
    }

    private async Task<FileResponseDto> GetFile(FileCoreDto file, CancellationToken cancellationToken)
    {
        var url = await _cloudStorageManager.GenerateUrl(
            file.VirtualDirectory,
            file.FilePath,
            cancellationToken);

        using var httpClient = _httpClientFactory.CreateClient();
        var encryptedContent = await httpClient.GetByteArrayAsync(new Uri(url), cancellationToken);

        var content = _payslipEncryptor.Decrypt(encryptedContent);

        return new FileResponseDto(file.ContentType, content);
    }

    internal sealed record Query(string FileId) : IQuery<FileResponseDto>;
}
