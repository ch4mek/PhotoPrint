using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;
using Modsen.Portal.Notification.UseCases.Dto;
using Modsen.Portal.Storage.Contract;

namespace Modsen.Portal.Notification.UseCases.Core.Composers;

internal interface IInquiryTeamsNotificationComposer
{
    Task<NotificationEntity[]> Compose(
        NotificationsCreatedEvent message, CancellationToken cancellationToken);
}

internal class InquiryTeamsNotificationComposer : IInquiryTeamsNotificationComposer
{
    private const string DefaultAvatarFileName = "DefaultAvatar.base64";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    private readonly INotificationDbContext _notificationDbContext;
    private readonly IConfiguration _configuration;
    private readonly IStorageContract _storageContract;

    public InquiryTeamsNotificationComposer(
        INotificationDbContext notificationDbContext,
        IConfiguration configuration,
        IStorageContract storageContract)
    {
        _notificationDbContext = notificationDbContext;
        _configuration = configuration;
        _storageContract = storageContract;
    }

    public async Task<NotificationEntity[]> Compose(
        NotificationsCreatedEvent message, CancellationToken cancellationToken)
    {
        if (!message.NotificationChannel.HasFlag(NotificationChannels.Teams))
        {
            return [];
        }

        using var doc = JsonDocument.Parse(message.Arguments);
        var root = doc.RootElement;

        var notificationInfo = JsonSerializer.Deserialize<NotificationInfo>(root.GetRawText(), _jsonOptions);

        if (notificationInfo == null)
        {
            return [];
        }

        var recipients = await _notificationDbContext.NotificationUsers
            .AsNoTracking()
            .Where(x => message.UserIds.Contains(x.UserId))
            .ToArrayAsync(cancellationToken);
        var user = await _notificationDbContext.NotificationUsers
            .AsNoTracking()
            .Where(x => x.UserId == notificationInfo.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        var avatarUri = await GetAvatarUri(user?.AvatarFileId, cancellationToken);

        var iconFileName = GetIconImageFileName(notificationInfo.InquiryType);
        var inquiryIconUri = await LoadImageAsync(iconFileName, cancellationToken);

        var userNotificationBodyDto = new UserNotificationBodyDto
        {
            Id = user!.UserId,
            AvatarFileId = user.AvatarFileId ?? string.Empty,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AvatarUri = avatarUri ?? string.Empty,
        };
        var teamsNotifications = recipients.Select(recipient => new NotificationEntity
        {
            UserId = recipient.UserId,
            Arguments = JsonSerializer.Serialize(new InquiryNotificationBodyDto
            {
                Id = notificationInfo.InquiryId ?? 0,
                DateFrom = notificationInfo.DateFrom ?? DateOnly.MinValue,
                DateTo = notificationInfo.DateTo ?? DateOnly.MinValue,
                TemplateName = notificationInfo.TemplateName ?? string.Empty,
                InquiryType = notificationInfo.InquiryType.GetDescription(),
                NotificationType = message.NotificationType.GetDescription(),
                RecipientEmail = recipient.Email,
                User = userNotificationBodyDto,
                Uri = GetRequestUri(notificationInfo.InquiryId ?? 0),
                InquiryIconUri = inquiryIconUri ?? string.Empty,
                Message = notificationInfo.Comment ?? string.Empty,
            }, JsonSerializerOptions.Web),
            NotificationChannel = NotificationChannels.Teams,
            NotificationType = message.NotificationType,
            Sent = true,
            Read = false,
            IsDeleted = false,
        }).ToArray();

        return teamsNotifications;
    }

    private async Task<string?> GetAvatarUri(string? avatarFileId, CancellationToken cancellationToken)
    {
        var avatarUri = string.Empty;

        if (!string.IsNullOrEmpty(avatarFileId))
        {
            avatarUri = await _storageContract.GetFileBase64Async(avatarFileId, ApplicationFileSize.SmallSquare, cancellationToken);
        }

        if (string.IsNullOrEmpty(avatarUri))
        {
            avatarUri = await LoadImageAsync(DefaultAvatarFileName, cancellationToken);
        }

        return avatarUri;
    }

    private static async Task<string?> LoadImageAsync(string fileName, CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = string.Join('.', assembly.GetName().Name, "Core", "Resources", fileName);
        var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            return string.Empty;
        }

        using var streamReader = new StreamReader(stream);
        var iconDataUrl = await streamReader.ReadToEndAsync(cancellationToken);
        return iconDataUrl;
    }

    private static string GetIconImageFileName(InquiryType inquiryType)
    {
        inquiryType = inquiryType == InquiryType.Unknown ? InquiryType.OtherType : inquiryType;
        return string.Join('.', inquiryType, "base64");
    }

    private Uri GetRequestUri(int inquiryId)
    {
        var baseUri = _configuration["AppSettings:MicrosoftTeamsBotApi:RedirectBaseUrl"];
        return new Uri($"{baseUri}?inquiryId={inquiryId}");
    }
}

internal sealed record NotificationInfo
{
    public int? UserId { get; init; }
    public int? InquiryId { get; init; }
    public string? TemplateName { get; init; }
    public InquiryType InquiryType { get; init; }
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
    public string? Comment { get; init; }
}
