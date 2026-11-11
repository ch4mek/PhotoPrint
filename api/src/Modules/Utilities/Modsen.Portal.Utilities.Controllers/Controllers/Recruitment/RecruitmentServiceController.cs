using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.Jwt;
using Modsen.Portal.Core.Authentication.RecruitmentKey;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Contract.Models;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.Utilities.Controllers.Recruitment;

namespace Modsen.Portal.Utilities.Controllers.Controllers.Recruitment;

[AllowAnonymous]
[Area("[Recruitment] Recruitment")]
[Route("api/recruitment-service")]
[ServiceFilter(typeof(RecruitmentKeyHeaderAttribute))]
[EnableRateLimiting("RecruitmentLimiter")]
public class RecruitmentServiceController : Controller
{
    private readonly IIdentityContract _identityContract;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IStorageContract _storageContract;

    private readonly IDataProtectionService _dataProtector;

    private readonly ITokenService _tokenService;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;

    public RecruitmentServiceController(
        IIdentityContract identityContract,
        ITeamBoardContract teamBoardContract,
        IStorageContract storageContract,
        IDataProtectionFactory dataProtectionFactory,
        ITokenService tokenService,
        IAuthenticationTokenContext authenticationTokenContext)
    {
        _identityContract = identityContract;
        _teamBoardContract = teamBoardContract;
        _storageContract = storageContract;

        ArgumentNullException.ThrowIfNull(dataProtectionFactory);
        _dataProtector = dataProtectionFactory.Create("Recruitment");

        _tokenService = tokenService;
        _authenticationTokenContext = authenticationTokenContext;
    }

    [HttpPost("token-payload")]
    public async Task<ActionResult<TokenPayloadDto>> GetTokenPayload([FromBody] GetTokenPayloadRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cancellationToken = HttpContext.RequestAborted;
        var payload = await GetTokenPayload(request.Token, cancellationToken);

        return Ok(payload);
    }

    [HttpGet("departments")]
    public async Task<ActionResult<List<DepartmentClassificationDto>>> GetDepartments()
    {
        var cancellationToken = HttpContext.RequestAborted;
        var departments = await _identityContract.GetDepartmentsClassificator(cancellationToken);

        return Ok(departments);
    }

    [HttpGet("english-levels")]
    public async Task<ActionResult<List<DepartmentClassificationDto>>> GetEnglishLevels()
    {
        var cancellationToken = HttpContext.RequestAborted;
        var englishLevels = await _teamBoardContract.GetEnglishLevels(cancellationToken);

        return Ok(englishLevels);
    }

    [HttpGet("seniority-levels")]
    public async Task<ActionResult<List<DepartmentClassificationDto>>> GetSeniorityLevels()
    {
        var cancellationToken = HttpContext.RequestAborted;
        return Ok(new
        {
            HardSkill = await _teamBoardContract.GetSeniorityLevels(SeniorityLevelScopes.HardSkill, cancellationToken),
            SoftSkill = await _teamBoardContract.GetSeniorityLevels(SeniorityLevelScopes.SoftSkill, cancellationToken),
            User = await _teamBoardContract.GetSeniorityLevels(SeniorityLevelScopes.TeamBoardUser, cancellationToken),
        });
    }

    [HttpGet("hard-skills")]
    public async Task<ActionResult<List<DepartmentClassificationDto>>> GetHardSkills()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var hardSkills = await _teamBoardContract.GetHardSkills(cancellationToken);

        return Ok(hardSkills);
    }

    [HttpGet("hard-skill-groups")]
    public async Task<ActionResult<List<DepartmentClassificationDto>>> GetHardSkillGroups()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var hardSkillGroups = await _teamBoardContract.GetHardSkillGroups(cancellationToken);

        return Ok(hardSkillGroups);
    }

    [HttpGet("soft-skills")]
    public async Task<ActionResult<List<DepartmentClassificationDto>>> GetSoftSkills()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var softSkills = await _teamBoardContract.GetSoftSkills(cancellationToken);

        return Ok(softSkills);
    }

    /// <summary>
    /// Get encrypted string
    /// </summary>
    /// <param name="value">string to encrypt</param>
    /// <returns>encrypted string</returns>
    [HttpGet("encrypt")]
    public ActionResult<string> GetEncryptedString(string value)
    {
        var encrypted = _dataProtector.Encrypt(value);

        return Ok(encrypted);
    }

    /// <summary>
    /// Get decrypted string
    /// </summary>
    /// <param name="value">string to decrypt</param>
    /// <returns>decrypted string</returns>
    [HttpGet("decrypt")]
    public ActionResult<string> GetDecryptedString(string value)
    {
        var decrypted = _dataProtector.Decrypt(value);

        return Ok(decrypted);
    }

    [HttpGet("user-sync-events")]
    public async Task<ActionResult<UserShortDto[]>> GetUserSyncEvents()
    {
        var events = await _identityContract.GetUserShortInfo(HttpContext.RequestAborted);
        return Ok(events);
    }

    /// <summary>
    /// Get Server file by id
    /// Allowed file source: Recruitment
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <returns>Base64 Data URI string</returns>
    [HttpGet("application-file/{fileId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetFile([FromRoute] string fileId)
    {
        ArgumentNullException.ThrowIfNull(fileId);

        var response = await _storageContract.GetFileBase64Async(fileId, FileSource.Recruitment, HttpContext.RequestAborted);

        if (response is null)
        {
            throw new EntityNotFoundException("File does not exist");
        }

        return Ok(response);
    }

    /// <summary>
    /// Upload file to server
    /// </summary>
    /// <param name="file">File form to upload</param>
    /// <param name="purpose">Purpose of the uploaded file (defines how the file will be processed and used</param>
    /// <returns>Uploaded file info</returns>
    [HttpPost("application-file")]
    [ProducesResponseType(typeof(RecruitmentFileOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecruitmentFileOutput>> UploadFile(IFormFile file, [FromQuery] ApplicationFilePurpose purpose = ApplicationFilePurpose.General)
    {
        ArgumentNullException.ThrowIfNull(file);

        await using var memoryStream = new MemoryStream();
        await using var openReadStream = file.OpenReadStream();
        await openReadStream.CopyToAsync(memoryStream, HttpContext.RequestAborted);

        var fileContent = memoryStream.ToArray();

        var fileId = await _storageContract.UploadFile(new UploadFileDto
        {
            FileContent = fileContent,
            FileName = file.FileName,
            FileSize = fileContent.Length,
            UserId = null,
            AvailableTill = null,
            ContentType = file.ContentType,
            FileSource = FileSource.Recruitment,
            FilePurpose = purpose
        }, HttpContext.RequestAborted);

        return Ok(new RecruitmentFileOutput
        {
            FileId = fileId,
            FileSize = fileContent.Length,
            FileName = file.FileName
        });
    }

    /// <summary>
    /// Upload files to server.
    /// Max total body size: 320 MB.
    /// Multipart form data supported only
    /// </summary>
    /// <returns>Uploaded files info</returns>
    [RequestSizeLimit(1024 * 1024 * 20 * 16)]
    [HttpPost("application-file/multiply")]
    [ProducesResponseType(typeof(List<RecruitmentFileOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<RecruitmentFileOutput>>> UploadFilesAsStream()
    {
        if (string.IsNullOrEmpty(Request.ContentType) || !Request.ContentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataAppException("Invalid content type");
        }

        var contentType = Request.GetTypedHeaders().ContentType;

        ArgumentNullException.ThrowIfNull(contentType);

        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

        if (string.IsNullOrWhiteSpace(boundary))
        {
            throw new InvalidDataException("Missing content-type boundary.");
        }

        var files = await _storageContract.UploadFiles(new UploadFilesDto
        {
            Boundary = boundary,
            ContentStream = Request.Body,
            FileSource = FileSource.Recruitment,
            FilePurpose = ApplicationFilePurpose.General
        }, HttpContext.RequestAborted);

        var output = files
            .Select(x => new RecruitmentFileOutput
            {
                FileId = x.FileId,
                FileSize = x.FileSize,
                FileName = x.FileName
            })
            .ToList();

        return Ok(output);
    }

    /// <summary>
    /// Delete file from server
    /// Allowed file source: Recruitment
    /// </summary>
    /// <param name="fileId">File id to delete</param>
    /// <returns></returns>
    [HttpDelete("application-file/{fileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteFile([FromRoute] string fileId)
    {
        ArgumentNullException.ThrowIfNull(fileId);

        await _storageContract.DeleteFile(fileId, FileSource.Recruitment, HttpContext.RequestAborted);

        return Ok();
    }

    private async Task<TokenPayloadDto> GetTokenPayload(string token, CancellationToken cancellationToken)
    {
        var (isValid, claims) = _tokenService.GetClaimsFromToken(token);
        var isTokenRevoked = await _authenticationTokenContext.IsTokenRevoked(token, cancellationToken);
        var userId = GetClaimValue(ClaimTypes.NameIdentifier);

        return new TokenPayloadDto
        {
            IsValidToken = isValid,
            IsTokenRevoked = isTokenRevoked,
            UserId = userId,
            UserEmail = GetClaimValue(ClaimTypes.Email),
            UserFirstName = GetClaimValue(ClaimTypes.GivenName),
            UserLastName = GetClaimValue(ClaimTypes.Name),
            UserDepartmentId = GetClaimValue(PortalClaimTypeNames.DepartmentId),
            UserResourceGroupId = GetClaimValue(PortalClaimTypeNames.ResourceGroupId),
            UserRole = GetClaimValue(ClaimTypes.Role),

            Claims = await _identityContract.GetUserClaimsByGroup(RecruitmentClaimTypeNames.GroupId, int.Parse(userId!, CultureInfo.InvariantCulture), cancellationToken)
        };

        string? GetClaimValue(string type) => claims.Where(x => x.Type == type).Select(x => x.Value).FirstOrDefault();
    }
}
